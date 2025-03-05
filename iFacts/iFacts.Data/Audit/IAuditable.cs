namespace iFacts.Data.Audit;

public interface IAuditable
{
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { set; get; }

    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { set; get; }

    public int Version { get; set; }
}
