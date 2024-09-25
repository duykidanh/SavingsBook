using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace SavingsBook.Infrastructure.Authentication;

[CollectionName("Users")]
public class ApplicationUser : MongoIdentityUser<Guid>
{
    public string FullName { get; set; }
    public string IdCardNumber { get; set; }
    public string Address { get; set; }
}