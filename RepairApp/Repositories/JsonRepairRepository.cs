using System.Text.Json;
using System.Text.Json.Serialization;
using RepairApp.Models;

namespace RepairApp.Repositories;

public class JsonRepairRepository : IRepairRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public JsonRepairRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<List<RepairOrder>> GetAllAsync()
    {
        if (!File.Exists(_filePath))
            return new List<RepairOrder>();
        var json = await File.ReadAllTextAsync(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new List<RepairOrder>();
        return JsonSerializer.Deserialize<List<RepairOrder>>(json, _options)
            ?? new List<RepairOrder>();
    }

    public async Task<RepairOrder?> GetByIdAsync(Guid id)
    {
        var orders = await GetAllAsync();
        return orders.FirstOrDefault(x => x.Id == id);
    }

    public async Task AddAsync(RepairOrder order)
    {
        var orders = await GetAllAsync();
        orders.Add(order);
        await SaveAllAsync(orders);
    }

    public async Task UpdateAsync(RepairOrder order)
    {
        var orders = await GetAllAsync();
        var index = orders.FindIndex(x => x.Id == order.Id);
        if (index >= 0)
        {
            orders[index] = order;
            await SaveAllAsync(orders);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var orders = await GetAllAsync();
        orders.RemoveAll(x => x.Id == id);
        await SaveAllAsync(orders);
    }

    private async Task SaveAllAsync(List<RepairOrder> orders)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
        var json = JsonSerializer.Serialize(orders, _options);
        await File.WriteAllTextAsync(_filePath, json);
    }
}