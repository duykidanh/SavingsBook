using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SavingsBook.Application.Contracts.SavingBook;

namespace SavingsBook.HostApi.Utility;

public class BalanceUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public BalanceUpdateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var savingBookService = scope.ServiceProvider.GetRequiredService<ISavingBookService>();

                await savingBookService.UpdateBalancesAsync();
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
