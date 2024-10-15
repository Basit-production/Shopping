using Ahmed_mart.Models.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ahmed_mart.DbContexts.v1
{
    public class SqlDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SqlDbContext(
           DbContextOptions<SqlDbContext> dbContextOptions,
           IHttpContextAccessor httpContextAccessor) : base(dbContextOptions)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #region Table
        public DbSet<Audit> Audit { get; set; }
        public DbSet<AuditDetails> AuditDetails { get; set; }
        public DbSet<Admin>Admin { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<UserTracker> UserTracker { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Otp> Otp { get; set; }
        public DbSet<SmtpDetails> SmtpDetails { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Prefixes> Prefixes { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<StoreType> StoreType { get; set; }
        public DbSet<PaymentGatewayConfiguration> PaymentGatewayConfiguration { get; set; }
        //public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Products> Products { get; set; }   
        public DbSet<ProductOptions> ProductOptions { get; set; }   
        public DbSet<ProductOptionDetails> ProductOptionDetails { get; set; }   
        public DbSet<ProductAttributes> ProductAttributes { get; set; }   
        public DbSet<RelatedProducts> RelatedProducts { get; set; }   
        public DbSet<ProductImages> ProductImages { get; set; }   
        public DbSet<Attributes> Attributes { get; set; }   
        public DbSet<StockStatus> StockStatus { get; set; }   
        public DbSet<Customers> Customers { get; set; }   
        public DbSet<CustomerAddresses> CustomerAddresses { get; set; }   
        public DbSet<CustomerUsers> CustomerUsers { get; set; }  
        public DbSet<Coupons> Coupons { get; set; }  
        public DbSet<CustomerCoupons> CustomerCoupons { get; set; }  
        public DbSet<OrderDetails> OrderDetails { get; set; }  
        public DbSet<OrderHistory> OrderHistory { get; set; }  
        public DbSet<OrderOptionDetails> OrderOptionDetails { get; set; }  
        public DbSet<OrderPayments> OrderPayments { get; set; }  
        public DbSet<Orders> Orders { get; set; }  
        public DbSet<OrdersStatus> OrdersStatus { get; set; }  
        public DbSet<PriceList> PriceList { get; set; }  
        public DbSet<PriceListDetails> PriceListDetails { get; set; }  
 
        
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        private int GetUserId() => 1;//int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AuditChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AuditChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AuditChanges()
        {
            var entries = ChangeTracker.Entries().ToList();

            foreach (var entry in entries)
            {
                if (entry.Entity is IEntityBase entity && entry.State == EntityState.Modified)
                {
                    var httpContext = _httpContextAccessor.HttpContext;
                    var audit = CreateAudit(entity, httpContext);
                    AddAuditDetails(entry, audit);
                    Audit.Add(audit);
                    audit.RecordVersion = GetNextRecordVersion(audit);
                }
            }
        }

        private Audit CreateAudit(IEntityBase entity, HttpContext httpContext)
        {
            return new Audit
            {
                HttpMethod = httpContext?.Request.Method,
                Url = $"{httpContext?.Request.Host}{httpContext?.Request.Path}",
                EntityName = entity.GetType().Name,
                State = Entry(entity).State.ToString(),
                EntityID = entity.ID,
                ReasonForChange = "N/A",
                CreatedBy = GetUserId(),
                CreatedDate = DateTime.UtcNow
            };
        }

        private void AddAuditDetails(EntityEntry entry, Audit audit)
        {
            foreach (var property in entry.OriginalValues.Properties)
            {
                var originalValue = entry.OriginalValues[property];
                var currentValue = entry.CurrentValues[property];
                if (!Equals(originalValue, currentValue))
                {
                    var auditDetail = new AuditDetails
                    {
                        PropertyName = property.Name,
                        OriginalValue = originalValue?.ToString(),
                        CurrentValue = currentValue?.ToString()
                    };
                    audit.AuditDetails.Add(auditDetail);
                }
            }
        }

        private int GetNextRecordVersion(Audit audit)
        {
            var recordVersion = Audit.Where(
                    x => x.EntityName == audit.EntityName &&
                    x.EntityID == audit.EntityID)
                    .Max(x => (int?)x.RecordVersion) ?? 0;
            return ++recordVersion;
        }
    }
}
