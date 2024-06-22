using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlow;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddKafka(kafka => kafka
                   .UseConsoleLog()
                   .AddCluster(cluster => cluster
                       .WithBrokers(new[] { configuration["Kafka:Broker"] })
                        .WithSchemaRegistry(config => config.Url = configuration["Kafka:SchemaRegistry"])
               .AddProducer(configuration["Kafka:AbandonedProducer"], producer => producer
                   .DefaultTopic(configuration["Kafka:AbandonedProducer"])
                    .AddMiddlewares(middlewares =>
                       middlewares.AddSchemaRegistryAvroSerializer(new AvroSerializerConfig { SubjectNameStrategy = SubjectNameStrategy.Topic })
               )
               )
               ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
