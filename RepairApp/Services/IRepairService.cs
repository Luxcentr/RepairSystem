using RepairApp.Models;

namespace RepairApp.Services;

public interface IRepairService
{
    Task<List<RepairOrder>> GetAllAsync();
    Task<RepairOrder?> GetByIdAsync(Guid id);
    Task<RepairOrder> CreateAsync(RepairOrder order);
    Task<bool> UpdateAsync(Guid id, RepairOrder order);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ChangeStatusAsync(Guid id, RepairStatus status);
}