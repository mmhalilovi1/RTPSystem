using Domain.Enums;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices
{
    public class PositionExpirationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PositionExpirationService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public PositionExpirationService(IServiceScopeFactory scopeFactory, ILogger<PositionExpirationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CloseExpiredPositionsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška prilikom zatvaranja isteklih pozicija.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task CloseExpiredPositionsAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var expiredPositions = await context.Positions
                .Where(p => p.Status == PositionStatus.Open && p.Deadline < DateTime.UtcNow)
                .ToListAsync(stoppingToken);

            if (expiredPositions.Count == 0) return;

            foreach (var position in expiredPositions)
            {
                position.Close();
            }

            await context.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Zatvoreno {Count} isteklih pozicija.", expiredPositions.Count);
        }
    }
}