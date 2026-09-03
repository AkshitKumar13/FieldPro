using SQLite;

namespace FieldPro.Models;

public class WorkOrder
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Priority { get; set; } = "Medium";

    public string Status { get; set; } = "Pending";

    public DateTime ScheduledDate { get; set; }

    public string Description { get; set; } = string.Empty;
}
