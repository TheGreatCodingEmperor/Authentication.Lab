using System.Text;
using Authentication.Lab.Repository;
using Authentication.Lab.Repository.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder (args);

// Add services to the container.
builder.Services.AddAuthentication (x => {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer (o => {
        var Key = Encoding.UTF8.GetBytes (builder.Configuration["JWT:Key"]);
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey (Key)
        };
    });
builder.Services.AddScoped<IJWTManagerRepository, JWTManagerRepository> ();
builder.Services.AddScoped<JWTTokenRepository> ();

builder.Services.AddControllers ();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (opt => {
    opt.AddSecurityDefinition ("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
    });
    opt.AddSecurityRequirement (new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build ();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment ()) {
    app.UseSwagger ();
    app.UseSwaggerUI ();
}

// using (var scope = app.Services.CreateScope ()) {
//     JWTTokenRepository? jWTTokenRepository = scope.ServiceProvider.GetRequiredService<JWTTokenRepository> ();
// }

app.UseAuthentication ().UseMiddleware<AuthMiddleware> (); // This need to be added	
app.UseAuthorization ();

app.UseHttpsRedirection ();

app.UseAuthorization ();

app.MapControllers ();

app.Run ();