using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using TCFiapConsumerDeleteContact.API;
using TCFiapConsumerDeleteContact.API.Model;
using TechChallenge.SDK.Persistence;

namespace TCFiapConsumerDeleteContact.Tests.IntegrationTests
{
    [TestFixture]
    public class WorkerIntegrationTests
    {
        private IHost _host;
        private IServiceScope _scope;

        [SetUp]
        public async Task SetUp()
        {
            try
            {
                _host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        var contactRepositoryMock = new Mock<IContactRepository>();
                        services.AddSingleton(contactRepositoryMock.Object);


                        services.AddMassTransit(x =>
                        {
                            x.AddConsumer<RemoveContactConsumer>();
                            x.UsingInMemory((context, cfg) =>
                            {
                                cfg.ConfigureEndpoints(context);
                            });
                        });
                        services.AddLogging(builder => {
                            builder.AddConsole();
                            builder.SetMinimumLevel(LogLevel.Debug);
                        });
                    })
                    .Build();

                await _host.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

                _scope = _host.Services.CreateScope();
                _scope.ServiceProvider.GetRequiredService<IBus>();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Erro durante o SetUp: {ex.Message}");
            }
        }

        [Test]
        public async Task RemoveContactConsumer_DeveRegistrarLogAoReceberMensagem()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var loggerMock = new Mock<ILogger<RemoveContactConsumer>>();
            var contactRepositoryMock = new Mock<IContactRepository>();
            var consumer = new RemoveContactConsumer(loggerMock.Object, contactRepositoryMock.Object);

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

            _scope?.Dispose();
        }
    }
}
