using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(op =>
{
    op.SetKebabCaseEndpointNameFormatter();
    op.SetInMemorySagaRepositoryProvider();

    var assembly = typeof(Program).Assembly;

    //op.AddConsumer<BookUpdatedConsumer>();
    op.AddSagas(assembly);
    op.AddActivities(assembly);

    op.UsingRabbitMq((context, cfg) =>
    {
        var rabbit = builder.Configuration.GetSection("RabbitMQ");
        cfg.Host(rabbit["Host"], "/", h =>
        {
            h.Username(rabbit["Username"]);
            h.Password(rabbit["Password"]);
        });
        //cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
