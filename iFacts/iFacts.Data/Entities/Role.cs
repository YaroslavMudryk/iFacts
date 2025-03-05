using iFacts.Data.Audit;

namespace iFacts.Data.Entities;

public class Role : AuditableBaseModelWithIdentity<int>
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public List<UserRole> UserRoles { get; set; }
    public List<RoleClaim> RoleClaims { get; set; }
}
