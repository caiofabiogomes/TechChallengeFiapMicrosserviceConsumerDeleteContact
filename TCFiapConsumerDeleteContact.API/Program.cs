using MassTransit;
using TCFiapConsumerDeleteContact.API;
using TechChallenge.SDK;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        Task.Delay(15000).Wait();

        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_DATABASE") ??
                               hostContext.Configuration.GetConnectionString("DefaultConnection");

        if(string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException("Database connection cannot be empty.");


        var envHostRabbitMqServer = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

        services.RegisterSdkModule(connectionString);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<RemoveContactConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(envHostRabbitMqServer);

                cfg.ReceiveEndpoint("delete-contact-queue", e =>
                {
                    e.ConfigureConsumer<RemoveContactConsumer>(context);
                });
            });
        });

        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .Build();

await host.RunAsync();
