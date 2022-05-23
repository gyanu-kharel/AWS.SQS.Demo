using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using AwsSQS.Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AwsSQS.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IOptions<AwsSecrets> _config;

        // More info at https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/welcome.html

        public WeatherForecastController(IOptions<AwsSecrets> config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task Post(WeatherForecast data)
        {
            var credentials = new BasicAWSCredentials(_config.Value.SqsAccessKey, _config.Value.SqsSecretKey);
            var sqsClient = new AmazonSQSClient(credentials, RegionEndpoint.USEast1);

            var request = new SendMessageRequest()
            {
                QueueUrl = _config.Value.SqsURl,
                MessageBody = JsonSerializer.Serialize(data),
            };

            _ = await sqsClient.SendMessageAsync(request);
        }
    }
}