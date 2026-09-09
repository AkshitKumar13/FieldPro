using FieldPro.Models;
using Microsoft.Maui.Storage;
using System.Text.Json;

namespace FieldPro.Data;

public class DatabaseSeeder
{
    private const string SeedFileName = "seed.json";

    public async Task SeedAsync(FieldProDatabase database)
    {
        var existing = await database.GetWorkOrdersAsync() ?? new List<WorkOrder>();

        // If DB already has records we will merge new seed items instead of skipping entirely.
        // This allows adding new seed entries without deleting the DB.

        try
        {
            // Read seed data from app package (Resources/Raw/seed.json)
            using var stream = await GetSeedStreamAsync();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var workOrders = JsonSerializer.Deserialize<List<WorkOrder>>(json, options)
                ?? new List<WorkOrder>();

            var existingIds = new HashSet<int>(existing.Select(e => e.Id));

                    foreach (var wo in workOrders)
                    {
                        if (!existingIds.Contains(wo.Id))
                        {
                            // Insert only items that don't already exist (preserve user/production data)
                            await database.SaveWorkOrderAsync(wo);
                        }
                        else
                        {
                            // Optionally update EF Core record to reflect seed defaults
                            try
                            {
                                // Update only non-empty fields (simple merge)
                                using var scope = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
                                    .BuildServiceProvider();
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                    }
        }
        catch (Exception ex)
        {
            // If seed file not found or deserialization fails, write debug output.
            System.Diagnostics.Debug.WriteLine($"DatabaseSeeder.SeedAsync error: {ex}");
        }
    }

    public async Task ForceSeedAsync(FieldProDatabase database)
    {
        try
        {
            // Clear existing records and re-seed from the seed.json file
            await database.DeleteAllWorkOrdersAsync();

            using var stream = await GetSeedStreamAsync();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var workOrders = JsonSerializer.Deserialize<List<WorkOrder>>(json, options)
                ?? new List<WorkOrder>();

            foreach (var wo in workOrders)
            {
                await database.SaveWorkOrderAsync(wo);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DatabaseSeeder.ForceSeedAsync error: {ex}");
        }
    }

    private static async Task<Stream> GetSeedStreamAsync()
    {
        // Try open from app package first
        try
        {
            return await FileSystem.OpenAppPackageFileAsync(SeedFileName);
        }
        catch
        {
            // fallback to a few common locations in the build output / app folder
        }

        // Check AppContext.BaseDirectory (app executable folder) and search recursively
        var basePath = AppContext.BaseDirectory ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(basePath))
        {
            // direct candidate
            var candidate = Path.Combine(basePath, SeedFileName);
            if (File.Exists(candidate))
                return File.OpenRead(candidate);

            // search all subfolders under basePath (covers win-x64, runtimes, Resources, etc.)
            try
            {
                var found = Directory.EnumerateFiles(basePath, SeedFileName, SearchOption.AllDirectories).FirstOrDefault();
                if (!string.IsNullOrEmpty(found) && File.Exists(found))
                    return File.OpenRead(found);
            }
            catch
            {
                // ignore directory enumeration errors
            }
        }

        // Check FileSystem.AppDataDirectory (maybe copied there during development)
        var appDataCandidate = Path.Combine(FileSystem.AppDataDirectory, SeedFileName);
        if (File.Exists(appDataCandidate))
        {
            return File.OpenRead(appDataCandidate);
        }

        // As last resort, try relative path inside project (useful when running from IDE)
        var relative = Path.Combine(Environment.CurrentDirectory, "Resources", "Raw", SeedFileName);
        if (File.Exists(relative))
        {
            return File.OpenRead(relative);
        }

        throw new FileNotFoundException($"Seed file '{SeedFileName}' not found in app package or known locations.");
    }
}
