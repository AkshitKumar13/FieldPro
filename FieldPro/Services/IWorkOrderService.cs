using FieldPro.Models;

namespace FieldPro.Services;

public interface IWorkOrderService
{
    Task<List<WorkOrder>> GetWorkOrdersAsync();

    Task<WorkOrder?> GetWorkOrderByIdAsync(int id);
    Task SaveWorkOrderAsync(WorkOrder workOrder);

}