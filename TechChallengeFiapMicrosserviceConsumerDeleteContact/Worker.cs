using MassTransit;

namespace TechChallengeFiapMicrosserviceConsumerDeleteContact
{
    public class Worker : BackgroundService
    {
        private readonly IBusControl _busControl;
        private readonly ILogger<Worker> _logger;

        public Worker(IBusControl busControl, ILogger<Worker> logger)
        {
            _busControl = busControl;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Worker Service iniciado para consumir mensagens as {DateTime.Now}...");

            await _busControl.StartAsync(stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _busControl.StopAsync(cancellationToken);
            _logger.LogInformation($"Worker Service finalizado as {DateTime.Now}.");
        }
    }
}
