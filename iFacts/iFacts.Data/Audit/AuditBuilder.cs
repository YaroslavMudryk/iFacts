using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace iFacts.Data.Audit;

public class AuditBuilder
{
    private EntityState _state;
    private readonly AuditItem _audit = new();
    private bool _softDeleted;
    public static AuditBuilder NewDefaultAudit() => new();

    public AuditBuilder On(EntityState state)
    {
        _state = state;
        _audit.Event = GetEventType();
        return this;
    }

    public AuditBuilder SoftDeleted(bool softDeleted)
    {
        _softDeleted = softDeleted;
        return this;
    }

    public AuditBuilder With(Action<AuditItem> action)
    {
        action(_audit);
        return this;
    }

    public AuditBuilder WithChanges(EntityEntry entityEntry)
    {
        _audit.Changes = EntityParser.ParseChanges(entityEntry);
        return this;
    }

    public AuditItem Build() => _audit;

    private string GetEventType()
    {
        return _state switch
        {
            EntityState.Deleted => AuditEvent.Delete,
            EntityState.Modified when _softDeleted => AuditEvent.SoftDelete,
            EntityState.Modified => AuditEvent.Update,
            _ => AuditEvent.Create
        };
    }
}
