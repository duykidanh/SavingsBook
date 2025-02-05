using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using SavingsBook.Application.Contracts.Common;

namespace SavingsBook.Application.Contracts.Authentication;

[CollectionName("Roles")]
public class ApplicationRole : MongoIdentityRole<ObjectId>, IAuditedEntityDto<ObjectId>
{
    public DateTime CreationTime { get; set; }
    public ObjectId? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public bool IsActive { get; set; }
    public ObjectId? LastModifierId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    public ObjectId? DeleterId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Keyword { get; set; } = string.Empty;
    ObjectId IAuditedEntityDto<ObjectId>.CreatorId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    ObjectId IAuditedEntityDto<ObjectId>.LastModifierId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    ObjectId IAuditedEntityDto<ObjectId>.DeleterId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
