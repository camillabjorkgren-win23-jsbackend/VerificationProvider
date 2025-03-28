using Azure.Messaging.ServiceBus;
using VerificationProvider.Models;

namespace VerificationProvider.Services;
public interface IVerificationService
{
    EmailRequest GenerateEmailRequestWhenNewUser();
    string GenerateCode();
    EmailRequest GenerateEmailRequest(VerificationRequest verificationRequest, string code);
    string GenerateServiceBusEmailRequest(EmailRequest emailRequest);
    Task<bool> SaveVerificationRequest(VerificationRequest verificationRequest, string code);
    VerificationRequest UnpackVerificationRequest(ServiceBusReceivedMessage message);
}