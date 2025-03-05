using iFacts.Data.Audit;

namespace iFacts.Data.Entities;

public class Claim : AuditableBaseModelWithIdentity<int>
{
    public string Type { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public List<RoleClaim> RoleClaims { get; set; }
    public List<AppClaim> AppClaims { get; set; }
}
