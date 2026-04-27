using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APISportFoodStore.Models;

public partial class FoodStoreDbContext : DbContext
{
    public FoodStoreDbContext()
    {
    }

    public FoodStoreDbContext(DbContextOptions<FoodStoreDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<AgentPresence> AgentPresences { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChatEvent> ChatEvents { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<ChatSession> ChatSessions { get; set; }

    public virtual DbSet<DeliveryTimeSlot> DeliveryTimeSlots { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewImage> ReviewImages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    public virtual DbSet<UserCard> UserCards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-FC54EJD\\SQLEXPRESS;Initial Catalog=FoodStoreDB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.IdCart).HasName("PK__Cart__3B7B33F2F5A81AFD");

            entity.ToTable("Cart");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("PK__Categori__CBD747065C06E895");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ChatEvent>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("PK__ChatEven__E0B2AF39E868D0D0");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Chat).WithMany(p => p.ChatEvents)
                .HasForeignKey(d => d.ChatId)
                .HasConstraintName("FK__ChatEvent__ChatI__2180FB33");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.IdMessage).HasName("PK__ChatMess__47AAF3048638118F");

            entity.ToTable(tb => tb.HasTrigger("TRG_Cleanup_OldChatMessages"));

            entity.Property(e => e.Body).IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EditedAt).HasColumnType("datetime");
            entity.Property(e => e.MessageType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("text");
            entity.Property(e => e.SenderRole)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Chat).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.ChatId)
                .HasConstraintName("FK__ChatMessa__ChatI__1AD3FDA4");

        });

        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.IdChat).HasName("PK__ChatSess__3817F38CB1D020F8");

            entity.Property(e => e.ClosedAt).HasColumnType("datetime");
            entity.Property(e => e.LastMessageAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("open");


        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.IdProductImage).HasName("PK__ProductI__13A69FC984BED088");

            entity.Property(e => e.AltText)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DeliveryTimeSlot>(entity =>
        {
            entity.HasKey(e => e.IdDeliverySlot).HasName("PK__Delivery__51F28B961EA73682");

            entity.Property(e => e.TimeRange)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.IdFavorite).HasName("PK__Favorite__39DCEE465847518B");


        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.IdManufacturer).HasName("PK__Manufact__5A3E39D35EF39353");

            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("PK__Orders__C38F300968CEEEF9");

            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            

        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.IdOrderDetail).HasName("PK__OrderDet__D8E06C51DA5C5321");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");



        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.IdOrderStatus).HasName("PK__OrderSta__CFDB50465474B190");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Products__2E8946D4C439144B");

            entity.HasIndex(e => e.Article, "UQ__Products__4943444ADCB5A62A").IsUnique();

            entity.Property(e => e.Article)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CaloriesKcal).HasColumnType("decimal(6, 1)");
            entity.Property(e => e.CarbsG).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Composition)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.FatG).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProteinG).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VolumeOrWeight).HasColumnType("decimal(10, 2)");


        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.IdReview).HasName("PK__Reviews__BB56047DF5458292");

            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

        });

        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.HasKey(e => e.IdReviewImage).HasName("PK__ReviewIm__413887A121CDF10B");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewImages)
                .HasForeignKey(d => d.ReviewId)
                .HasConstraintName("FK__ReviewIma__Revie__0F624AF8");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PK__Roles__B436905477D6AFEB");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__Users__B7C9263814CB3977");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105342F11E399").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MiddleName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ResetToken)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ResetTokenExpires).HasColumnType("datetime");
            entity.Property(e => e.Surname)
                .HasMaxLength(200)
                .IsUnicode(false);

        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.IdAddress).HasName("PK__UserAddr__F1CFF37F69691671");

            entity.Property(e => e.Apartament)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CourierComment)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.House)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Street)
                .HasMaxLength(100)
                .IsUnicode(false);

        });

        modelBuilder.Entity<UserCard>(entity =>
        {
            entity.HasKey(e => e.IdUserCard).HasName("PK__UserCard__50AD475A27E09912");

            entity.Property(e => e.CardNumber)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Cvv)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CVV");
            entity.Property(e => e.ExpiryDate)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
