using iFacts.Data.Audit;

namespace iFacts.Data.Entities;

public class App : AuditableBaseModelWithIdentity<int>
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ActiveFrom { get; set; }
    public DateTime? ActiveTo { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public List<AppClaim> AppClaims { get; set; }
}
