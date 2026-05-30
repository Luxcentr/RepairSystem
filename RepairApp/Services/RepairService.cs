using RepairApp.Models;
using RepairApp.Repositories;

namespace RepairApp.Services;

public class RepairService : IRepairService
{
    private readonly IRepairRepository _repository;

    public RepairService(IRepairRepository repository)
    {
        _repository = repository;
    }

    public Task<List<RepairOrder>> GetAllAsync() => _repository.GetAllAsync();

    public Task<RepairOrder?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);

    public async Task<RepairOrder> CreateAsync(RepairOrder order)
    {
        if (string.IsNullOrWhiteSpace(order.DeviceName))
            throw new ArgumentException("Название устройства обязательно.");
        if (string.IsNullOrWhiteSpace(order.ClientName))
            throw new ArgumentException("Имя клиента обязательно.");

        await _repository.AddAsync(order);
        return order;
    }

    public async Task<bool> UpdateAsync(Guid id, RepairOrder order)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return false;
        order.Id = id;
        await _repository.UpdateAsync(order);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return false;
        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ChangeStatusAsync(Guid id, RepairStatus status)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order is null) return false;
        order.ChangeStatus(status);
        await _repository.UpdateAsync(order);
        return true;
    }
}