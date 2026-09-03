using FieldPro.Data;
using FieldPro.Models;

namespace FieldPro.Services;

public class WorkOrderService : IWorkOrderService
{
    private readonly FieldProDatabase _database;

    public WorkOrderService(FieldProDatabase database)
    {
        _database = database;
    }

    public async Task<List<WorkOrder>> GetWorkOrdersAsync()
    {
        return await _database.GetWorkOrdersAsync();
    }

    public async Task<WorkOrder?> GetWorkOrderByIdAsync(int id)
    {
        return await _database.GetWorkOrderAsync(id);
    }

    public async Task SaveWorkOrderAsync(WorkOrder workOrder)
    {
        await _database.SaveWorkOrderAsync(workOrder);
    }
}
