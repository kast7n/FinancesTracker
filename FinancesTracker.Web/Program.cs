using FinancesTracker.Infrastructure.Data;
using FinancesTracker.Domain.Repositories;
using FinancesTracker.Infrastructure.Repositories;
using FinancesTracker.Application.Managers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// DbContext
builder.Services.AddDbContext<FinancesTrackerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository registrations
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Manager registrations
builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped(provider =>
    new TransactionManager(
        provider.GetRequiredService<ITransactionRepository>(),
        provider.GetRequiredService<IAccountRepository>(),
        provider.GetRequiredService<IMapper>()
    )
);

// AutoMapper registration
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Build the app
var app = builder.Build();

// Initialize and seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinancesTracker.Infrastructure.Data.FinancesTrackerDbContext>();
    FinancesTracker.Infrastructure.Data.DbInitializer.Initialize(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
