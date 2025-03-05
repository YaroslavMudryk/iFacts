namespace iFacts.Data.Audit;

public interface ISoftDeleted
{
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { set; get; }
}
