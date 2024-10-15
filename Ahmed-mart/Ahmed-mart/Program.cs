using Ahmed_mart.DbContexts.v1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using System.Text;
using Ahmed_mart.Repository.v1.GenericRepository;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Ahmed_mart.Configurations.v1;
using Microsoft.Extensions.Options;
using FluentValidation.AspNetCore;
using Ahmed_mart.Repository.v1;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ahmed_mart.Middlewares.v1;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Ahmed_mart.Hubs.v1;
using Ahmed_mart.Services.v1.AdminAuthService;
using Ahmed_mart.Services.v1.FileService;
using Ahmed_mart.Services.v1.StudentService;
using Ahmed_mart.Services.v1.EmailService;
using Ahmed_mart.Services.v1.UserAuthService;
using Ahmed_mart.Services.v1.SmsService;
using Ahmed_mart.Filter.v1;
using Ahmed_mart.Services.v1.CategoryService;
using Ahmed_mart.Services.v1.StoreService;
using Ahmed_mart.Services.v1.StoreTypeService;
using Ahmed_mart.Services.v1.ProductsService;
using Ahmed_mart.Services.v1.CustomerAuthService;
using Ahmed_mart.Services.v1.OrderHistoryService;
using Ahmed_mart.Services.v1.OrdersService;
using Ahmed_mart.Services.v1.OrdersStatusService;
using Ahmed_mart.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddMongoDb(builder.Configuration.GetValue<string>("MongoDbConnectionSettings:ConnectionString")!);

builder.Services.AddHealthChecksUI(options =>
{
    options.AddHealthCheckEndpoint("Healthcheck Api", "/hc");
}).AddInMemoryStorage();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 25 * 1024 * 1024;//Bytes=Megabytes×1024×1024
});

builder.Services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 25 * 1024 * 1024);

//Db Conection
builder.Services.AddDbContext<SqlDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("Jwt:key").Value!)),
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
            ValidateAudience = false,
            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
        };
    });

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });
    options.AddSlidingWindowLimiter("sliding", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(10);
        options.SegmentsPerWindow = 2;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });
    options.AddTokenBucketLimiter("token", options =>
    {
        options.TokenLimit = 100;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        options.TokensPerPeriod = 20;
        options.AutoReplenishment = true;
    });
    options.AddConcurrencyLimiter("concurrency", options =>
    {
        options.PermitLimit = 10;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Add services to the container.
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddMemoryCache();
//register servicelayer
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IUserAuthService, UserAuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStoreTypeService, StoreTypeService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<ICustomerAuthService, CustomerAuthService>();
builder.Services.AddScoped<IOrderHistoryService, OrderHistoryService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IOrdersStatusService, OrdersStatusService>();
//builder.Services.AddScoped<SmsApiClient>();
builder.Services.AddScoped<SmsApiClient>(provider =>
{
    var baseUrl = builder.Configuration["Sms:ProviderUrl"]; // Use builder.Configuration
    return new SmsApiClient(baseUrl);
});


//MongoDb
builder.Services.Configure<MongoDbConnectionSettings>(builder.Configuration.GetSection(nameof(MongoDbConnectionSettings)));
builder.Services.AddSingleton(serviceProvider =>
{
    var mongoDbConnectionSettings = serviceProvider.GetRequiredService<IOptions<MongoDbConnectionSettings>>().Value;
    var mongoDbContext = new MongoDbContext(mongoDbConnectionSettings.ConnectionString, mongoDbConnectionSettings.DatabaseName);
    return mongoDbContext;
});
//builder.Services.AddScoped<IStudentService, StudentService>();

//builder.Services.AddControllers();
builder.Services.AddControllers().AddFluentValidation(options =>
{
    options.ImplicitlyValidateChildProperties = true;
    options.ImplicitlyValidateRootCollectionElements = true;
    options.RegisterValidatorsFromAssemblyContaining<Program>();
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>();

        var errorResponse = new ServiceResponse<object>
        {
            Data = default,
            Success = false,
            StatusCode = HttpStatusCode.BadRequest,
            Message = "Validation errors.",
            Errors = errors.ToList()
        };

        return new ObjectResult(errorResponse)
        {
            StatusCode = (int)errorResponse.StatusCode
        };
    };
});

//To Avoid possible object cycle was detected errors And circular reference issue and avoid the serialization errors
builder.Services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);

//for filter
builder.Services.AddControllers(config =>
{
    config.Filters.Add(new UserTrackingFilterAsync());
});

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme. Example: (\"Bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    builder.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options => options.UIPath = "/hc-dashboard");

app.UseRateLimiter();

// Add the global exception handling middleware
app.UseMiddleware<ExceptionMiddleware>();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        foreach (var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{apiVersionDescription.GroupName}/swagger.json", apiVersionDescription.GroupName.ToUpperInvariant());
        }
    });
}

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("notification-hub");

app.UseHangfireDashboard();

app.Run();

//EntityFrmeworkCore\Add-migration initial -Context SqlDbContext
