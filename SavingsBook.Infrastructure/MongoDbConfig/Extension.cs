using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using SavingsBook.Domain.Common;
using SavingsBook.Infrastructure.Repositories;

namespace SavingsBook.Infrastructure.MongoDbConfig;

public static class Extension
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeSerializer(BsonType.String));
        
        services.AddSingleton(serviceProvider =>
        {
            var configurator = serviceProvider.GetService<IConfiguration>();
            
            var mongoClient = new MongoClient(configurator["MongoDbSettings:ConnectionStrings"]);
            var database = mongoClient.GetDatabase(configurator["MongoDbSettings:DatabaseName"]);
            return database;
        });
        return services;
    }

    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
        where T : AuditedAggregateRoot<Guid>
    {
        services.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new Repository<T>(database, collectionName);
        });
        return services;
    }

    public static IServiceCollection InitMongoCollections(this IServiceCollection services)
    {
        services
            .AddMongo();
            // .AddMongoRepository<Store>("Store")
            // .AddMongoRepository<StoreCategory>("StoreCategory");
        
        
        
        return services;
    }
}