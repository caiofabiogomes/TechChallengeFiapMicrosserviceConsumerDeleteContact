using MassTransit;
using TCFiapConsumerDeleteContact.API;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<RemoveContactConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

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