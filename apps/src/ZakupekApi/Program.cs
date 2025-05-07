global using FluentValidation;
using ErrorOr;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Polly;
using ZakupekApi;
using ZakupekApi.Db.Data;
using ZakupekApi.Wrapper.Abstraction.Auth;
using ZakupekApi.Wrapper.Auth;
using ZakupekApi.Wrapper.Contract.Auth.Validation;
using ZakupekApi.Wrapper.Contract.ShoppingLists.OpenRouter;
using ZakupekApi.Wrapper.ShoppingList;

var bld = WebApplication.CreateBuilder();

var jwtSettings = bld.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException();

var connectionString = bld.Configuration.GetConnectionString("DefaultConnection");

bld.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions
            .EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null)
            .CommandTimeout(30)
    ));

bld.Services.Configure<OpenRouterSettings>(
    bld.Configuration.GetSection("OpenRouter"));

bld.Services.AddHttpClient<ShoppingListService>()
    .AddTransientHttpErrorPolicy(p => 
        p.WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

bld.Services
    .AddAuthenticationJwtBearer(s => s.SigningKey = secretKey)
    .Configure<JwtCreationOptions>(o => o.SigningKey = secretKey)
    .AddAuthorization()
    .AddFastEndpoints(options => { options.Assemblies = [typeof(LoginRequestValidator).Assembly]; })
    .SwaggerDocument()
    .Scan(scan => scan
        .FromAssembliesOf(typeof(AuthService), typeof(IAuthService))
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
        .AsImplementedInterfaces()
        .WithScopedLifetime());

var app = bld.Build();
app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(
            c =>
            {
                c.Errors.UseProblemDetails();
                c.Endpoints.Configurator =
                    ep =>
                    {
                        if (ep.ResDtoType.IsAssignableTo(typeof(IErrorOr)))
                        {
                            ep.DontAutoSendResponse();
                            ep.PostProcessor<ResponseSender>(Order.After);
                            ep.Description(
                                b => b.ClearDefaultProduces()
                                    .Produces(200, ep.ResDtoType.GetGenericArguments()[0])
                                    .ProducesProblemDetails());
                        }
                    };
            })
    .UseSwaggerGen();

app.Run();
