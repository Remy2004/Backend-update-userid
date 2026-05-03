using Aoun.API.Data;
using Aoun.API.Middleware;
using Aoun.BLL.Interfaces;
using Aoun.BLL.Interfaces.Campaign;
using Aoun.BLL.Interfaces.Donation;
using Aoun.BLL.Interfaces.Email;
using Aoun.BLL.Interfaces.Favorite;
using Aoun.BLL.Services;
using Aoun.BLL.Services.Admin;
using Aoun.BLL.Services.Charity;
using Aoun.BLL.Services.Chat;
using Aoun.BLL.Services.Email;
using Aoun.BLL.Services.Profile;
using Aoun.BLL.Services.Zakat;
using Aoun.DAL.Data;
using Aoun.DAL.Repositories;
using Aoun.DAL.Repositories.Campaigns;
using Aoun.DAL.Repositories.Case;
using Aoun.DAL.Repositories.Donation;
using Aoun.DAL.Repositories.Favorite;
using Aoun.DAL.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// 1. Database Configuration
// ==========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=.;Database='Aoun Charity Platform';Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    )
);

// ==========================
// 2. Identity Configuration
// ==========================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;

    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ==========================
// 3. Controllers + JSON
// ==========================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        // 🔥 السطر ده هو اللي بيخلي Swagger يعرض الـ Enum كقائمة (Donor, Charity)
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
// ==========================
// 4. JWT Authentication
// ==========================
var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? throw new InvalidOperationException("JWT Secret is missing!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "AounApi",
        ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "AounAppUsers",

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        RoleClaimType = ClaimTypes.Role
    };
});

// ==========================
// 5. Authorization
// ==========================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// ==========================
// 6. Business Services (BLL)
// ==========================
builder.Services.AddBllServices(builder.Configuration);

builder.Services.AddHttpClient<AISmartService>();
builder.Services.AddHttpClient<MetalPriceService>();
builder.Services.AddHttpClient<PaymobService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAdminService, RealAdminService>();
builder.Services.AddScoped<ICharityService, RealCharityService>();
builder.Services.AddScoped<IProfileService, RealProfileService>();
builder.Services.AddScoped<ZakatService>();
builder.Services.AddScoped<PaymobService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<CampaignStateService>();

builder.Services.AddScoped<ICaseService, CaseService>();
builder.Services.AddScoped<ICaseRepository, CaseRepository>();

builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();

builder.Services.AddScoped<IDonationService, DonationService>();
builder.Services.AddScoped<IDonationRepository, DonationRepository>();

builder.Services.AddScoped<IFavoritesRepository, FavoritesRepository>();
builder.Services.AddScoped<IFavoritesService, FavoritesService>();

// ==========================
// 7. General API Services
// ==========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        p => p.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ==========================
// 8. Swagger + JWT
// ==========================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Aoun API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Example: Bearer eyJhbGciOi..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ==========================
// 9. Stripe Config
// ==========================
Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// ==========================
// HTTP Pipeline
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
        app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aoun API v1");
        c.EnablePersistAuthorization(); // 🔥 ميزة حفظ التوكن
        c.InjectStylesheet("/swagger-theme.css"); 
        c.InjectJavascript("/swagger-theme.js"); 
    });
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// Static Files (Images)
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==========================
// 10. Database Migrations & Seeding
// ==========================
using (var scope = app.Services.CreateScope())
{
var services = scope.ServiceProvider;

try
{
var context = services.GetRequiredService<ApplicationDbContext>();
var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

// 🔥 التعديل الجذري: ده بيعمل Update-Database أوتوماتيك أول ما المشروع يشتغل
await context.Database.MigrateAsync();

// 🔥 بينده على الـ Seed براحته عشان يضيف الأدمن والبيانات الأساسية
await Aoun.API.Data.DbInitializer.SeedAsync(context, userManager, roleManager);
}
catch (Exception ex)
{
var logger = services.GetRequiredService<ILogger<Program>>();
logger.LogError(ex, "An error occurred during database migration or seeding.");
}
}

app.Run();

public partial class Program { }