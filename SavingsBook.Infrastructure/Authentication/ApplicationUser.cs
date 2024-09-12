using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace SavingsBook.Infrastructure.Authentication;

[CollectionName("Users")]
public class ApplicationUser : MongoIdentityUser<Guid>
{
    
}