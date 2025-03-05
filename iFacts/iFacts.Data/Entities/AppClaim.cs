using iFacts.Data.Audit;

namespace iFacts.Data.Entities;

public class AppClaim : AuditableBaseModelWithIdentity<int>
{
    public int AppId { get; set; }
    public App App { get; set; }

    public int ClaimId { get; set; }
    public Claim Claim { get; set; }
}
