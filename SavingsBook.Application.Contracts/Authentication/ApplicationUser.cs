using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SavingsBook.Application.Contracts.Common;

namespace SavingsBook.Application.Contracts.Authentication;

[CollectionName("Users")]
public class ApplicationUser : MongoIdentityUser<ObjectId>, IAuditedEntityDto<ObjectId>
{
    [BsonElement("Username")]
    public override string UserName { get; set; } = string.Empty;
    [BsonElement("Password")]
    public string PasswordHash { get; set; } = string.Empty;
    public ObjectId?[] RoleIds { get; set; }
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

    public string RefreshToken { get; set; } = string.Empty;
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public string ResetPasswordToken { get; set; } = string.Empty;
    public DateTime? ResetPasswordExpiresAt { get; set; }

    ObjectId IAuditedEntityDto<ObjectId>.CreatorId
    {
        get => CreatorId ?? ObjectId.Empty;
        set => CreatorId = value;
    }
    ObjectId IAuditedEntityDto<ObjectId>.LastModifierId
    {
        get => LastModifierId ?? ObjectId.Empty;
        set => LastModifierId = value;
    }
    ObjectId IAuditedEntityDto<ObjectId>.DeleterId
    {
        get => DeleterId ?? ObjectId.Empty;
        set => DeleterId = value;
    }
}