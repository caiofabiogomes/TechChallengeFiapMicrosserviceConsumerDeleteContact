using MassTransit;
using TCFiapConsumerDeleteContact.API.Model;
using TechChallenge.SDK.Persistence;

namespace TCFiapConsumerDeleteContact.API
{
    public class RemoveContactConsumer : IConsumer<RemoveContactMessage>
    {
        private readonly ILogger<RemoveContactConsumer> _logger;
        private readonly IContactRepository _contactRepository;

        public RemoveContactConsumer(ILogger<RemoveContactConsumer> logger, IContactRepository contactRepository)
        {
            _logger = logger;
            _contactRepository = contactRepository;
        }

        public async Task Consume(ConsumeContext<RemoveContactMessage> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Recebida solicitação para deletar o contato com ID: {message.ContactId}");

            var contact = await _contactRepository.GetByIdAsync(message.ContactId);
            if(contact == null)
            {
                _logger.LogWarning($"Contato {message.ContactId} não encontrado!");
                return;
            }

            await _contactRepository.DeleteAsync(contact);

            await Task.Delay(500);
            _logger.LogInformation($"Contato {message.ContactId} removido com sucesso!");
        }
    }
}
