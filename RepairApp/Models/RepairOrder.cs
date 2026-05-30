using System.Text.Json.Serialization;

namespace RepairApp.Models;

public class RepairOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DeviceName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }

    [JsonInclude]
    public RepairStatus Status { get; private set; } = RepairStatus.New;

    public string Priority { get; set; } = "Normal";
    public decimal? Cost { get; set; }

    public void ChangeStatus(RepairStatus newStatus)
    {
        Status = newStatus;
        if (newStatus == RepairStatus.Done)
            CompletedAt = DateTime.Now;
    }
}