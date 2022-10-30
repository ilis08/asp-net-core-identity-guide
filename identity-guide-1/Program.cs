using identity_guide_1.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
                .AddRazorRuntimeCompilation();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", opts =>
{
    opts.Cookie.Name = "MyCookieAuth";
    opts.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("MustBelongToHRDepartment",
        policy => policy.RequireClaim("Department", "HR"));

    opts.AddPolicy("HRManagerOnly", policy => policy
            .RequireClaim("Manager")
            .Requirements.Add(new HRManagerProbationRequirement(3)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7127/");
});

var app = builder.Build();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
