using MassTransit;
using TCFiapConsumerDeleteContact.API;
using TechChallenge.SDK;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        Task.Delay(15000).Wait();

        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_DATABASE") ??
                               hostContext.Configuration.GetConnectionString("DefaultConnection");

        //var envHostRabbitMqServer = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

        if(string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException("Database connection cannot be empty.");

        services.RegisterSdkModule(connectionString);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<RemoveContactConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                // cfg.Host(envHostRabbitMqServer);

                cfg.Host("rabbitmq://localhost", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("delete-contact-queue", e =>
                {
                    e.ConfigureConsumer<RemoveContactConsumer>(context);

                    // DLQ Exchange 
                    e.SetQueueArgument("x-dead-letter-exchange", "delete-contact-dlx-exchange");
                    e.SetQueueArgument("x-dead-letter-routing-key", "delete-contact-dlx");
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
