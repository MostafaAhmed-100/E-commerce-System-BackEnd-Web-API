using FluentValidation;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.Design;
using System.Text;
using WebApplication1.Authentication;
using WebApplication1.BackgroundJobs.OrderJobs;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Filters;
using WebApplication1.Middlewares;
using WebApplication1.PaymentGateway.Services;
using WebApplication1.Repository.GenericRepository;
using WebApplication1.Repository.SpecificRepository.AddressRepository;
using WebApplication1.Repository.SpecificRepository.BuyerRepository;
using WebApplication1.Repository.SpecificRepository.CartRepository;
using WebApplication1.Repository.SpecificRepository.CategoryRepository;
using WebApplication1.Repository.SpecificRepository.CategoryRepository.Interface;
using WebApplication1.Repository.SpecificRepository.CouponRepository;
using WebApplication1.Repository.SpecificRepository.LoyaltyTransactionRepository;
using WebApplication1.Repository.SpecificRepository.OrderRepository;
using WebApplication1.Repository.SpecificRepository.ProductRepository;
using WebApplication1.Repository.SpecificRepository.RefreshTokenRepository;
using WebApplication1.Repository.SpecificRepository.ReviewRepository;
using WebApplication1.Repository.SpecificRepository.SavedCardRepository;
using WebApplication1.Repository.SpecificRepository.SellerRepository;
using WebApplication1.Repository.SpecificRepository.WishlistItemRepository;
using WebApplication1.Repository.SpecificRepository.WishlistsRepository;
using WebApplication1.Repository.UnitOfWork;
using WebApplication1.Services;
using WebApplication1.Services.AccountService;
using WebApplication1.Services.AddressService;
using WebApplication1.Services.AuthService;
using WebApplication1.Services.CartService;
using WebApplication1.Services.CategoryService;
using WebApplication1.Services.CouponService;
using WebApplication1.Services.EmailService;
using WebApplication1.Services.Implementation;
using WebApplication1.Services.Interface;
using WebApplication1.Services.OrderService;
using WebApplication1.Services.ProductService;
using WebApplication1.Services.WishlistService;
using WebApplication1.Services.WishlistService.cs;
using WebApplication1.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

builder.Services.AddIdentity<User, Role>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 7;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 10;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<JWT>(builder.Configuration.GetSection("Jwt"));
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<ISavedCardRepository, SavedCardRepository>();
builder.Services.AddScoped<IBuyerRepository, BuyerRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<ILoyaltyTransactionRepository, LoyaltyTransactionRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IWishlistsRepository, WishlistsRepository>();
builder.Services.AddScoped<IWishlistItemRepository, WishlistItemRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderBackgroundJobs, OrderBackgroundJobs>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ISavedCardService, SavedCardService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IReviewRepository , ReviewRepository>();
builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddAutoMapper(configAction => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


builder.Services.AddHttpClient();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "SQL Server",
        timeout: TimeSpan.FromSeconds(3),
        tags: new[] { "database" }
    );

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "AuthPolicy", configureOptions =>
    {
        configureOptions.PermitLimit = 5;
        configureOptions.Window = TimeSpan.FromMinutes(1);
        configureOptions.QueueLimit = 0;
    });
    options.AddConcurrencyLimiter(policyName: "CheckoutPolicy", configureOptions =>
    {
        configureOptions.PermitLimit = 1;
        configureOptions.QueueLimit = 0;
    });
    options.AddTokenBucketLimiter("BrowsingPolicy", configureOptions =>
    {
        configureOptions.TokenLimit = 100;
        configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
        configureOptions.TokensPerPeriod = 10;
        configureOptions.QueueLimit = 0;
    });
    options.AddTokenBucketLimiter("UserActivityPolicy", configureOptions =>
    {
        configureOptions.TokenLimit = 50;
        configureOptions.TokensPerPeriod = 5;
        configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
        configureOptions.QueueLimit = 0;
    });
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("V2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "E-Commerce API",
        Version = "V2"
    });
    options.OperationFilter<AcceptLanguageHeaderFilter>();

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();
var supportedCultures = new[] { "en", "ar" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.UseMiddleware<ExceptionMiddleware>();

app.Use(async (context, next) =>
{
    var watch = System.Diagnostics.Stopwatch.StartNew();
    context.Response.OnStarting(() =>
    {
        watch.Stop();
        context.Response.Headers.Append("X-Response-Time", $"{watch.ElapsedMilliseconds} ms");
        return Task.CompletedTask;
    });

    await next();
});

if (app.Environment.IsDevelopment())
{
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/V2/swagger.json", "My API V2");
    c.DisplayRequestDuration();
});
app.UseSerilogRequestLogging();
app.UseHangfireDashboard("/hangfire");
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
public class AcceptLanguageHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Description = "Select Language (en / ar)",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new Microsoft.OpenApi.Any.OpenApiString("en")
            }
        });
    }
}