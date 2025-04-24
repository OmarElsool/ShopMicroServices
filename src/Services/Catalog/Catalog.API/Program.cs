using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database");

var assembly = typeof(Program).Assembly;
//Add services to the containers
builder.Services.AddMediatR(config =>
{
    //register all services in project into MediatR library (this fix problem where library installed on BuildingBlocks and used on Catalog.API)
    config.RegisterServicesFromAssembly(assembly);
    //Register the BuildingBlock Behaviors(Validation) (Generic)
    config.AddOpenBehavior(typeof(ValidationBehaviors<,>)); // <,> means generic
    config.AddOpenBehavior(typeof(LoggingBehavior<,>)); 
});
//Inject FluentValidation
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(opt =>
{
    opt.Connection(connectionString!);
}).UseLightweightSessions();

if(builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>(); //data seeding in development mode

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString!);

var app = builder.Build();

//Configure the HTTP request pipeline
app.MapCarter();

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

//Hundle Exception Golabel (not better approach for microservices bcs we'll need to do it in every program.cs project use =>CustomExceptionHandler)
//app.UseExceptionHandler(exceptionHundlerApp =>
//{
//    exceptionHundlerApp.Run(async context =>
//    {
//        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
//        if(exception == null)
//            return;

//        var problemDetails = new ProblemDetails
//        {
//            Title = exception.Message,
//            Status = StatusCodes.Status500InternalServerError,
//            Detail = exception.StackTrace
//        };

//        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//        logger.LogError(exception, exception.Message);

//        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//        context.Response.ContentType = "application/problem+json";

//        await context.Response.WriteAsJsonAsync(problemDetails);
//    });
//});

app.UseExceptionHandler(options => { });

app.Run();
