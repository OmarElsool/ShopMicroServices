var builder = WebApplication.CreateBuilder(args);

//Add services to the containers

var app = builder.Build();

//Configure the HTTP request pipeline

app.Run();
