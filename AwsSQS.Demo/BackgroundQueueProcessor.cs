using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using AwsSQS.Demo.Models;
using Microsoft.Extensions.Options;

namespace AwsSQS.Demo
{
    public class BackgroundQueueProcessor : BackgroundService
    {
        private readonly IOptions<AwsSecrets> _config;
        public BackgroundQueueProcessor(IOptions<AwsSecrets> config)
        {
            _config = config;
        }

        // More info at https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/welcome.html
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Background Queue Processing Started At {DateTime.Now}");

            var credentials = new BasicAWSCredentials(_config.Value.SqsAccessKey, _config.Value.SqsSecretKey);
            var sqsClient = new AmazonSQSClient(credentials, RegionEndpoint.USEast1);

            while(!stoppingToken.IsCancellationRequested)
            {
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = _config.Value.SqsURl,
                    WaitTimeSeconds = 5,
                };

                var response = await sqsClient.ReceiveMessageAsync(request);

                foreach(var message in response.Messages)
                {
                    Console.WriteLine($"Message received at {DateTime.Now}");
                    Console.WriteLine(message.Body);

                    if (message.Body.Contains("Exception")) continue;

                    _ = await sqsClient.DeleteMessageAsync(_config.Value.SqsURl, message.ReceiptHandle);

                    Console.WriteLine("Message processed and removed from queue");
                }
            }
        }
    }
}
