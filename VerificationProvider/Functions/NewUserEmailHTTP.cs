using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using VerificationProvider.Models;
using VerificationProvider.Services;

namespace VerificationProvider.Functions
{
    public class NewUserEmailHTTP
    {
        private readonly ILogger<NewUserEmailHTTP> _logger;
        private readonly IVerificationService _verificationService;
        private readonly QueueClient _queueClient;


        public NewUserEmailHTTP(ILoggerFactory loggerFactory, IVerificationService verificationService)
        {
            _logger = loggerFactory.CreateLogger<NewUserEmailHTTP>();
            _verificationService = verificationService;
            string serviceBusConnection = Environment.GetEnvironmentVariable("ServiceBusConnection")!;
            string queueName = Environment.GetEnvironmentVariable("ServiceBusQueueNameEmail")!;
            _queueClient = new QueueClient(serviceBusConnection, queueName);
        }
        [Function("NewUserEmailHTTP")]
        public async Task<bool> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(body))
                return false;


            var verificationRequest = JsonConvert.DeserializeObject<VerificationRequest>(body.ToString());
            if (verificationRequest == null && string.IsNullOrEmpty(verificationRequest.Email))
                return false;

            var generateEmailRequest = _verificationService.GenerateEmailRequestWhenNewUser();
            if (generateEmailRequest == null)
                return false;

            try
            {
                var payload = _verificationService.GenerateServiceBusEmailRequest(generateEmailRequest);
                if (!string.IsNullOrEmpty(payload))
                {
                    var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                    {
                        Email = generateEmailRequest.To,
                    })));
                    await _queueClient.SendAsync(message);
                    _logger.LogInformation("Message sent to Service Bus queue");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : GenerateServiceBusEmailRequest.Run() :: {ex.Message}");

            }
            return false;

        }

    }
}