using RepairApp.Models;

namespace RepairApp.Repositories;

public interface IRepairRepository
{
    Task<List<RepairOrder>> GetAllAsync();
    Task<RepairOrder?> GetByIdAsync(Guid id);
    Task AddAsync(RepairOrder order);
    Task UpdateAsync(RepairOrder order);
    Task DeleteAsync(Guid id);
}