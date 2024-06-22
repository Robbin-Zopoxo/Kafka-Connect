using AutoMapper;
using FlatAbandonedCheckout;
using KafkaFlow.Producers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopifySharp;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IProducerAccessor producerAccessor;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IProducerAccessor producerAccessor, IConfiguration configuration, IMapper mapper)
        {
            _logger = logger;
            this.producerAccessor = producerAccessor;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task Get()
        {
            var items = new ShopifySharp.Checkout();
            string filePath = "Order.json";

            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<ShopifySharp.Checkout>(json);
            }
            var mappedItem=mapper.Map<AbandonedCheckout>(items);
            var producer = producerAccessor.GetProducer(configuration["Kafka:AbandonedProducer"]);
            await producer.ProduceAsync(configuration["Kafka:AbandonedProducer"], mappedItem);
        }

        
    }
}
