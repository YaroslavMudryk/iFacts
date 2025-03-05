using System.ComponentModel.DataAnnotations.Schema;

namespace iFacts.Data.Audit;

public class BaseModel
{
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}

public class BaseModelWithIdentity<TId> : BaseModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TId Id { get; set; }
}

public class AuditableBaseModelWithIdentity<TId> : BaseModelWithIdentity<TId>, IAuditable, IVersioning
{
    public int Version { get; set; }
}

public class AuditableSoftDeletedBaseModelWithIdentity<TId> : AuditableBaseModelWithIdentity<TId>, ISoftDeleted
{
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; }
}

public class SoftDeletedBaseModelWithIdentity<TId> : BaseModelWithIdentity<TId>, ISoftDeleted
{
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; }
}

public class BaseModelWithoutIdentity<TId> : BaseModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public TId Id { get; set; }
}

public class AuditableBaseModelWithoutIdentity<TId> : BaseModelWithoutIdentity<TId>, IAuditable, IVersioning
{
    public int Version { get; set; }
}

public class AuditableSoftDeletedBaseModelWithoutIdentity<TId> : AuditableBaseModelWithoutIdentity<TId>, ISoftDeleted
{
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; }
}

public class SoftDeletedBaseModelWithoutIdentity<TId> : BaseModelWithoutIdentity<TId>, ISoftDeleted
{
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; }
}
