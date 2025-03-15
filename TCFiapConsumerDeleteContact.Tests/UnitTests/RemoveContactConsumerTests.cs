using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using TCFiapConsumerDeleteContact.API;
using TechChallenge.SDK.Domain.Models;
using TechChallenge.SDK.Infrastructure.Message;
using TechChallenge.SDK.Infrastructure.Persistence;

namespace TCFiapConsumerDeleteContact.Tests.UnitTests
{
    [TestFixture]
    public class RemoveContactConsumerTests
    {
        private RemoveContactConsumer _consumer;
        private Mock<ILogger<RemoveContactConsumer>> _loggerMock;
        private Mock<IContactRepository> _contactRepositoryMock;
        private Mock<ConsumeContext<RemoveContactMessage>> _consumeContextMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<RemoveContactConsumer>>();
            _contactRepositoryMock = new Mock<IContactRepository>();
            _consumer = new RemoveContactConsumer(_loggerMock.Object, _contactRepositoryMock.Object);
            _consumeContextMock = new Mock<ConsumeContext<RemoveContactMessage>>();
        }

        [Test]
        public async Task Consume_WhenContactExists_ShouldDeleteContactAndLogSuccess()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var message = new RemoveContactMessage { ContactId = contactId };

            _consumeContextMock.Setup(c => c.Message).Returns(message);
            var fakeContact = new Contact { Id = contactId };
            _contactRepositoryMock.Setup(r => r.GetByIdAsync(contactId))
                .ReturnsAsync(fakeContact);

            // Act
            await _consumer.Consume(_consumeContextMock.Object);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Recebida solicitação para deletar o contato com ID: {contactId}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _contactRepositoryMock.Verify(r => r.DeleteAsync(fakeContact), Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Contato {contactId} removido com sucesso!")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task Consume_WhenContactDoesNotExist_ShouldLogWarningAndNotDeleteContact()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var message = new RemoveContactMessage { ContactId = contactId };

            _consumeContextMock.Setup(c => c.Message).Returns(message);
            _contactRepositoryMock.Setup(r => r.GetByIdAsync(contactId))
                .ReturnsAsync((Contact)null);

            // Act
            await _consumer.Consume(_consumeContextMock.Object);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Contato {contactId} não encontrado!")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _contactRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Contact>()), Times.Never);
        }


        [Test]
        public async Task Consume_WhenCalled_ShouldLogReceivedMessage()
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
