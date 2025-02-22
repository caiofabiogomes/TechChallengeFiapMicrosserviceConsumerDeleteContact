using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using TechChallengeFiapMicrosserviceConsumerDeleteContact;
using TechChallengeFiapMicrosserviceConsumerDeleteContact.Model;

namespace TCFiapConsumerDeleteContact.Tests.IntegrationTests
{
    [TestFixture]
    public class WorkerIntegrationTests
    {
        private IHost _host;
        private IBus _bus;
        private IServiceScope _scope;
        private ILogger<Worker> _logger;

        [SetUp]
        public async Task SetUp()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
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

                    services.AddLogging();
                    services.AddHostedService<Worker>();
                })
                .Build();

            await _host.StartAsync();

            _scope = _host.Services.CreateScope();
            _bus = _scope.ServiceProvider.GetRequiredService<IBus>();
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<Worker>>();
        }

        [Test]
        public async Task RemoveContactConsumer_DeveRegistrarLogAoReceberMensagem()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var loggerMock = new Mock<ILogger<RemoveContactConsumer>>();
            var consumer = new RemoveContactConsumer(loggerMock.Object);

            var consumeContextMock = new Mock<ConsumeContext<RemoveContactMessage>>();
            consumeContextMock.Setup(x => x.Message).Returns(new RemoveContactMessage { ContactId = contactId });

            // Act
            await consumer.Consume(consumeContextMock.Object);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Recebida solicitação para deletar o contato com ID: {contactId}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TearDown]
        public async Task TearDown()
        {
            await _host.StopAsync();

            _host.Dispose();
            _scope.Dispose();
        }
    }
}
