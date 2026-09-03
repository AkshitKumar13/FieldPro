using FieldPro.Models;
using Microsoft.Maui.Storage;
using SQLite;

namespace FieldPro.Data;

public class FieldProDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public FieldProDatabase()
    {
        var databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "fieldpro.db3");

        _database = new SQLiteAsyncConnection(databasePath);
    }

    public async Task InitializeAsync()
    {
        await _database.CreateTableAsync<WorkOrder>();
    }

    public async Task<List<WorkOrder>> GetWorkOrdersAsync()
    {
        return await _database
            .Table<WorkOrder>()
            .ToListAsync();
    }

    public async Task<WorkOrder?> GetWorkOrderAsync(int id)
    {
        return await _database
            .Table<WorkOrder>()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> SaveWorkOrderAsync(WorkOrder workOrder)
    {
        return await _database
            .InsertOrReplaceAsync(workOrder);
    }

    public async Task DeleteAllWorkOrdersAsync()
    {
        // DeleteAllAsync<T> removes all rows from the table T
        await _database.DeleteAllAsync<WorkOrder>();
    }
}
