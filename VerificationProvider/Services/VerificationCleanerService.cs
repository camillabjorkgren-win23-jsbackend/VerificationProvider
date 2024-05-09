using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VerificationProvider.Data.Context;
using VerificationProvider.Functions;

namespace VerificationProvider.Services;
public class VerificationCleanerService(ILogger<VerificationCleaner> logger, DataContext dataContext) : IVerificationCleanerService
{
    private readonly ILogger<VerificationCleaner> _logger = logger;
    private readonly DataContext _dataContext = dataContext;

    public async Task RemoveExpiredRecordsAsync()
    {
        try
        {
            var expired = await _dataContext.VerificationRequests.Where(x => x.ExpiryDate <= DateTime.Now).ToListAsync();
            _dataContext.RemoveRange(expired);
            await _dataContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationCleaner.RemoveExpiredRecordsAsync() :: {ex.Message}");
        }

    }
}

//https://youtu.be/E1h-SAgej08?t=4980
