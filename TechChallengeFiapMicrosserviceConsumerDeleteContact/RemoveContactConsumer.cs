using MassTransit;
using TechChallengeFiapMicrosserviceConsumerDeleteContact.Model;

namespace TechChallengeFiapMicrosserviceConsumerDeleteContact
{
    public class RemoveContactConsumer : IConsumer<RemoveContactMessage>
    {
        private readonly ILogger<RemoveContactConsumer> _logger;

        public RemoveContactConsumer(ILogger<RemoveContactConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RemoveContactMessage> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Recebida solicitação para deletar o contato com ID: {message.ContactId}");

            // TODO: Implementar a remoção do contato

            await Task.Delay(500);
            _logger.LogInformation($"Contato {message.ContactId} removido com sucesso!");
        }
    }
}
