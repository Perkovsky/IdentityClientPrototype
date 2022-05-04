using IdentityClientPrototype.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

#region Authentication

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("https://localhost:7100/");
        options.AddAudiences("postman");
        options.AddAudiences("anonymous");

        options.UseIntrospection()
            .SetClientId("postman")
            .SetClientSecret("postman-secret");

        //options.Configure(c =>
        //{
        //    c.ValidationType = OpenIddictValidationType.Introspection;
        //    //c.TokenValidationParameters.NameClaimType = OpenIddictConstants.Claims.Subject;
        //    //c.TokenValidationParameters.RoleClaimType = OpenIddictConstants.Claims.Role;
        //    ////c.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
        //    //c.TokenValidationParameters.RoleClaimTypeRetriever = (token, s) =>
        //    //{
        //    //    return "<some_text>";
        //    //};
        //});

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });
builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        //.RequireRole("Accounting")
        .RequireAssertion(context => context.HasRole("Accounting"))
        //.RequireAssertion(context =>
        //{
        //    return context.User.IsInRole("Accounting");
        //})
        //.RequireAssertion(context => context.User.HasScope("roles"))
        .Build();
});

#endregion

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test Identity Client", Version = "v1" });

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
