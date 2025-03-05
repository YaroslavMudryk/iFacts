using iFacts.Data.Audit;

namespace iFacts.Data.Entities;

public class PasswordHistory : AuditableBaseModelWithIdentity<int>
{
    public string PasswordHash { get; set; }
    public string Hint { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}
