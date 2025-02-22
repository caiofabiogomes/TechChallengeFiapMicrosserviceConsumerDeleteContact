using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using TechChallengeFiapMicrosserviceConsumerDeleteContact;
using TechChallengeFiapMicrosserviceConsumerDeleteContact.Model;

namespace TCFiapConsumerDeleteContact.Tests.UnitTests
{
    [TestFixture]
    public class RemoveContactConsumerTests
    {
        private RemoveContactConsumer _consumer;
        private Mock<ILogger<RemoveContactConsumer>> _loggerMock;
        private Mock<ConsumeContext<RemoveContactMessage>> _consumeContextMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<RemoveContactConsumer>>();
            _consumer = new RemoveContactConsumer(_loggerMock.Object);
            _consumeContextMock = new Mock<ConsumeContext<RemoveContactMessage>>();
        }

        [Test]
        public async Task Consume_DeveLogarMensagemCorretamente()
        {
            // Arrange
            var message = new RemoveContactMessage { ContactId = Guid.NewGuid() };
            _consumeContextMock.Setup(c => c.Message).Returns(message);

            // Act
            await _consumer.Consume(_consumeContextMock.Object);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Recebida solicitação para deletar o contato com ID: {message.ContactId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
