using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace APISportFoodStore.Controllers
{
    [Route("api/[controller]")]

    public class DatabaseController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _backupFolder;
        public DatabaseController(IConfiguration config, IWebHostEnvironment env)
        {
            _connectionString = config.GetConnectionString("con");

            _backupFolder = Path.Combine(env.ContentRootPath, "Backups");
        }

        [HttpGet("backup")]
        public async Task<IActionResult> CreateBackup()
        {
            string dbName = "FoodStoreDB";
            string fileName = $"{dbName}_{DateTime.Now:yyyyMMddHHmmss}.bak";
            string fullPath = Path.Combine(_backupFolder, fileName);

            try
            {
                if (!Directory.Exists(_backupFolder)) Directory.CreateDirectory(_backupFolder);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = $"BACKUP DATABASE [{dbName}] TO DISK = @path WITH FORMAT, INIT;";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@path", fullPath);

                await command.ExecuteNonQueryAsync();
                connection.Close();

                byte[] bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                System.IO.File.Delete(fullPath);

                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        
        }

        [HttpPost("restore")]
        public async Task<IActionResult> Restore(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Файл не выбран");

            string dbName = "FoodStoreDB";
            string tempPath = Path.Combine(_backupFolder, "temp_restore.bak");

            try
            {
                if (!Directory.Exists(_backupFolder)) Directory.CreateDirectory(_backupFolder);

                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }


                var masterConnectionString = new SqlConnectionStringBuilder(_connectionString)
                {
                    InitialCatalog = "master"
                }.ToString();

                using var connection = new SqlConnection(masterConnectionString);
                await connection.OpenAsync();

                var sql = $@"
            ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            RESTORE DATABASE [{dbName}] FROM DISK = '{tempPath}' WITH REPLACE;
            ALTER DATABASE [{dbName}] SET MULTI_USER;";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();

                return Ok(new { message = "Восстановление завершено" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Критическая ошибка восстановления: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(tempPath)) System.IO.File.Delete(tempPath);
            }
        }

    }
}
