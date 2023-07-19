using System.Text;
using Chat.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                //ValidIssuer = TokenConfig.TokenIssuer,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SSomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeyomeRandomKey"))
            };
        });

builder.Services.AddControllers();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.


app.MapControllers();

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
});

app.UseRouting();

app.UseAuthentication();    

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/hubs/chat");
    endpoints.MapDefaultControllerRoute();
});


app.Run();
