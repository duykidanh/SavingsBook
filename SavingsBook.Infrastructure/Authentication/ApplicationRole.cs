using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace SavingsBook.Infrastructure.Authentication;

[CollectionName("Roles")]
public class ApplicationRole: MongoIdentityRole<Guid>
{
    
}