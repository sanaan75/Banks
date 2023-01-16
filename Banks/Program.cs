using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ClosedXML.Excel;
using Framework;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using NetCore.AutoRegisterDi;
using Persistence;
using UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Persistence.AppContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:MainConnection"]);
});

builder.Services.AddDbContext<FileContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:FileConnection"]);
});

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.RegisterAssemblyPublicNonGenericClasses(GetAssembliesToBeRegisteredInIocContainer())
    .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

builder.Services.AddCors();

builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(150); });

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

//app.Map("/api", ApiCheck);

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
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
        typeof(FrameworkDummy).Assembly,
        typeof(WebDummy).Assembly,
    };
}

static void ApiCheck(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        try
        {
            StringValues headerValue;
            context.Request.Headers.TryGetValue("JiroToken", out headerValue);

            var headerValueResult = headerValue.FirstOrDefault();
            string Api_Key = "$Jiro" + DateTime.Now.Minute + DateTime.Now.Hour + "6342";

            if (headerValueResult == null || headerValueResult.Equals(Api_Key) == false)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }
            else
            {
                await context.Response.WriteAsync("Authorized");
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Error");
        }
    });
}