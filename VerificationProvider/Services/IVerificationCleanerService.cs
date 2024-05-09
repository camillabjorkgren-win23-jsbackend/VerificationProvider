
namespace VerificationProvider.Services;

public interface IVerificationCleanerService
{
    Task RemoveExpiredRecordsAsync();
}