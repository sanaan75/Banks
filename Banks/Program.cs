using System.Reflection;
using Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using NetCore.AutoRegisterDi;
using Persistence;
using UseCases;
using Web.Jwt;
using AppContext = Persistence.AppContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:MainConnection"]);
});

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.RegisterAssemblyPublicNonGenericClasses(GetAssembliesToBeRegisteredInIocContainer())
    .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(120); });

builder.Services.AddJwtServices(opt => opt.AllowUserAccountNo = AppSetting.AllowUserAccountNo);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
    options.ValueLengthLimit = int.MaxValue;
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});

app.Run();

Assembly[] GetAssembliesToBeRegisteredInIocContainer()
{
    return new[]
    {
        typeof(UseCasesDummy).Assembly,
        typeof(PersistenceDummy).Assembly,
        typeof(WebDummy).Assembly
    };
}