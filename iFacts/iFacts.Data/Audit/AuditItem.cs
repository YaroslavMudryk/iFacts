using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace iFacts.Data.Audit;

public class AuditItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string By { get; set; }
    public string Event { get; set; }
    public string ItemId { get; set; }
    public string ItemType { get; set; }
    public string TransactionId { get; set; }
    public List<PropertyInfo> Changes { get; set; }

    [NotMapped]
    public List<PropertyEntry> TempProperties { get; set; }
}
