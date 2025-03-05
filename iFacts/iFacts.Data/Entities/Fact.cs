using iFacts.Data.Audit;

namespace iFacts.Data.Entities;

public class Fact : AuditableBaseModelWithIdentity<int>
{
    public string Text { get; set; }
}
