using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Data.Context;
using VerificationProvider.Models;

namespace VerificationProvider.Services;
public class ValidateVerificationCodeService(ILogger<ValidateVerificationCodeService> logger, DataContext dataContext) : IValidateVerificationCodeService
{
    private readonly ILogger<ValidateVerificationCodeService> _logger = logger;
    private readonly DataContext _dataContext = dataContext;

    public async Task<bool> ValidateCodeAsync(ValidateRequest validateRequest)
    {
        try
        {
            var entity = await _dataContext.VerificationRequests.FirstOrDefaultAsync(x => x.Email == validateRequest.Email && x.Code == validateRequest.Code);
            if (entity != null)
            {
                _dataContext.VerificationRequests.Remove(entity);
                await _dataContext.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : ValidateVerificationCode.ValidateCodeAsync :: {ex.Message}");
        }
        return false;

    }

    public async Task<ValidateRequest> UnpackValidateRequestAsync(HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                var validateRequest = JsonConvert.DeserializeObject<ValidateRequest>(body);
                if (validateRequest != null)
                    return validateRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : ValidateVerificationCode.UnpackValidateRequestAsync :: {ex.Message}");
        }
        return null!;
    }
}
