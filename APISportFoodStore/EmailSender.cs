using APISportFoodStore.Models;
using Microsoft.Extensions.Options;
using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;
using System.Text;

namespace APISportFoodStore
{
    public sealed class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> settings)
            => _settings = settings.Value;

        public async Task SendOrderConfirmationAsync(
            string toEmail,
            string fullName,
            Order order,
            IEnumerable<(string Name, int Quantity, decimal Price)> items,
            string? paymentTokenOrUrl = null)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = $"Ваш заказ №{order.IdOrder} оформлен";

            // Генерируем строки таблицы товаров
            var itemsHtml = new StringBuilder();
            foreach (var item in items)
            {
                itemsHtml.Append($@"
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #eee;'>{WebUtility.HtmlEncode(item.Name)}</td>
                        <td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{item.Quantity} шт.</td>
                        <td style='padding: 8px; border-bottom: 1px solid #eee; text-align: right;'>{item.Price:0.##} ₽</td>
                    </tr>");
            }

            var builder = new BodyBuilder();
            builder.HtmlBody = $@"
<div style='font-family:Segoe UI,Arial,sans-serif;font-size:15px;line-height:1.5;color:#222'>
    <h2 style='margin-bottom:6px'>{_settings.Branding.Company}</h2>
    <p>Здравствуйте, <strong>{WebUtility.HtmlEncode(fullName)}</strong>!</p>
    <p>Спасибо за заказ! Мы уже начали его собирать.</p>

    <div style='background-color:#f9f9f9; padding: 15px; border-radius: 8px; margin: 20px 0;'>
        <p style='margin:0'><strong>Номер заказа:</strong> {order.IdOrder}</p>
        <p style='margin:0'><strong>Дата доставки:</strong> {order.DeliveryDate:dd.MM.yyyy}</p>
    </div>

    <h3 style='margin-bottom:10px;'>Детали заказа:</h3>

    <table style='border-collapse:collapse;width:100%;margin-top:10px'>
        <thead>
            <tr style='background:#f5f5f5'>
                <th style='padding:8px;border:1px solid #ddd;text-align:left'>Товар</th>
                <th style='padding:8px;border:1px solid #ddd;text-align:center'>Кол-во</th>
                <th style='padding:8px;border:1px solid #ddd;text-align:right'>Цена</th>
            </tr>
        </thead>
        <tbody>
            {itemsHtml}
        </tbody>
    </table>

    <p style='font-size:18px; margin-top:15px; text-align:right;'>
        <strong>Сумма: {order.TotalAmount:0.##} ₽</strong>
    </p>

    <hr style='margin-top:20px;border:none;border-top:1px solid #eee'/>
    <p style='font-size:13px;color:#666;margin:0'>
        По вопросам обращайтесь в поддержку:<br/>
        <a href='mailto:{_settings.Branding.SupportEmail}'>{_settings.Branding.SupportEmail}</a><br/>
        Телефон: {_settings.Branding.SupportPhone}
    </p>
</div>";

            msg.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Smtp.Host, _settings.Smtp.Port,
                _settings.Smtp.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            await smtp.AuthenticateAsync(_settings.Smtp.User, _settings.Smtp.Password);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }
    

    public async Task SendPasswordResetAsync(string toEmail, string fullName, string resetLink)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = "Сброс пароля — " + _settings.Branding.Company;

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
                    <div style='font-family:Segoe UI,Arial,sans-serif;font-size:15px;line-height:1.5;color:#222'>
                      <p>Здравствуйте, <strong>{WebUtility.HtmlEncode(fullName)}</strong>!</p>
                      <p>Вы запросили сброс пароля. Чтобы задать новый пароль, перейдите по ссылке:</p>
                      <p><a href='{resetLink}' target='_blank' style='display:inline-block;padding:10px 16px;border-radius:6px;background:#0d6efd;color:#fff;text-decoration:none'>Сбросить пароль</a></p>
                      <p style='color:#666'>Ссылка действует 1 час. Если вы не запрашивали сброс пароля — просто игнорируйте это письмо.</p>
                      <hr style='border:none;border-top:1px solid #eee;margin:16px 0' />
                      <p style='color:#666;font-size:12px'>Поддержка: <a href='mailto:{_settings.Branding.SupportEmail}'>{_settings.Branding.SupportEmail}</a> · {_settings.Branding.SupportPhone}</p>
                    </div>"
            };

            msg.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Smtp.Host, _settings.Smtp.Port,
                _settings.Smtp.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            await smtp.AuthenticateAsync(_settings.Smtp.User, _settings.Smtp.Password);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendManagerNewOrderNotificationAsync(
    Order order,
    string customerName,
    string customerEmail,
    string customerAddress,
    IEnumerable<(string Name, int Quantity, decimal Price)> items)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            msg.To.Add(MailboxAddress.Parse(_settings.ManagerEmail));
            msg.Subject = $"НОВЫЙ ЗАКАЗ №{order.IdOrder}";

            var itemsHtml = new StringBuilder();

            foreach (var item in items)
            {
                itemsHtml.Append($@"
            <tr>
                <td style='padding:8px;border:1px solid #ddd'>{WebUtility.HtmlEncode(item.Name)}</td>
                <td style='padding:8px;border:1px solid #ddd;text-align:center'>{item.Quantity}</td>
                <td style='padding:8px;border:1px solid #ddd;text-align:right'>{item.Price:0.##} ₽</td>
            </tr>");
            }

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
        <div style='font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#222'>
            <h2 style='color:#dc3545'>Поступил новый заказ!</h2>

            <p><strong>Номер заказа:</strong> {order.IdOrder}</p>
            <p><strong>Дата оформления:</strong> {DateTime.Now:dd.MM.yyyy HH:mm}</p>
            <p><strong>Дата доставки:</strong> {order.DeliveryDate:dd.MM.yyyy}</p>
            <p><strong>Сумма:</strong> {order.TotalAmount:0.##} ₽</p>

            <hr/>

            <h3>Данные клиента:</h3>
            <p><strong>Имя:</strong> {WebUtility.HtmlEncode(customerName)}</p>
            <p><strong>Email:</strong> {WebUtility.HtmlEncode(customerEmail)}</p>
            <p><strong>Адрес:</strong> {WebUtility.HtmlEncode(customerAddress)}</p>

            <h3>Состав заказа:</h3>

            <table style='border-collapse:collapse;width:100%'>
                <thead>
                    <tr style='background:#f5f5f5'>
                        <th style='padding:8px;border:1px solid #ddd'>Товар</th>
                        <th style='padding:8px;border:1px solid #ddd'>Кол-во</th>
                        <th style='padding:8px;border:1px solid #ddd'>Цена</th> 
                    </tr>
                    {itemsHtml}
                    </table>
<p style='margin-top:20px;color:#666'>Это автоматическое уведомление системы {_settings.Branding.Company}</p>
        </div>"
            };

            msg.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Smtp.Host, _settings.Smtp.Port,
                _settings.Smtp.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

            await smtp.AuthenticateAsync(_settings.Smtp.User, _settings.Smtp.Password);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }
        public async Task SendCourierAssignmentAsync(string courierEmail, string courierName, Order order, string deliveryAddress)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            msg.To.Add(MailboxAddress.Parse(courierEmail));
            msg.Subject = $"Новый заказ для доставки №{order.IdOrder}";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;font-size:15px;line-height:1.5;color:#222'>
                <h2 style='color:#0d6efd;'>Новый заказ назначен!</h2>
                <p>Здравствуйте, <strong>{WebUtility.HtmlEncode(courierName)}</strong>!</p>
                <p>Вам назначен новый заказ для доставки.</p>
                
                <div style='background-color:#f4f4f4; padding: 15px; border-left: 4px solid #0d6efd; margin: 20px 0;'>
                    <p style='margin:0'><strong>Номер заказа:</strong> {order.IdOrder}</p>
                    <p style='margin:0'><strong>Адрес доставки:</strong> {WebUtility.HtmlEncode(deliveryAddress)}</p>
                    <p style='margin:0'><strong>Дата доставки:</strong> {order.DeliveryDate:dd.MM.yyyy}</p>
                    <p style='margin:0'><strong>Сумма заказа:</strong> {order.TotalAmount:0.##} ₽</p>
                </div>

                <p>Пожалуйста, приступайте к выполнению как можно скорее.</p>
                <hr style='border:none;border-top:1px solid #eee;margin:16px 0' />
                <p style='font-size:12px;color:#666'>Это автоматическое уведомление от системы {_settings.Branding.Company}.</p>
            </div>"
            };

            msg.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Smtp.Host, _settings.Smtp.Port,
                _settings.Smtp.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            await smtp.AuthenticateAsync(_settings.Smtp.User, _settings.Smtp.Password);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }

    }

    public sealed class EmailSettings
    {
        public string FromName { get; set; } = "";
        public string ManagerEmail { get; set; } = "qwsaa.a@mail.ru";
        public string FromAddress { get; set; } = "";
        public SmtpSettings Smtp { get; set; } = new();
        public BrandingSettings Branding { get; set; } = new();
    }

    public sealed class SmtpSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool UseStartTls { get; set; } = true;
        public string User { get; set; } = "ekaterinazavdv@gmail.com";
        public string Password { get; set; } = "lveu uefy rsyy nhtq";
    }

    public sealed class BrandingSettings
    {
        public string Company { get; set; } = "MealExpress";
        public string SupportEmail { get; set; } = "";
        public string SupportPhone { get; set; } = "";
        public string SiteUrl { get; set; } = "";
    }

}

