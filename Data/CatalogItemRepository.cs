using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ServiceCatalogAPI.Models;

namespace ServiceCatalogAPI.Data
{
    public static class CatalogItemRepository
    {
        private static readonly string _jsonPath = "catalog.json";

        public static List<CatalogItem> LoadItems()
        {
            if (!File.Exists(_jsonPath))
                return new List<CatalogItem>();
            var json = File.ReadAllText(_jsonPath);
            var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("CatalogItems").EnumerateArray()
                .Select(e => JsonSerializer.Deserialize<CatalogItem>(e.GetRawText()))
                .ToList();
            return items;
        }

        public static void SaveItems(List<CatalogItem> items)
        {
            var obj = new { CatalogItems = items };
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_jsonPath, json);
        }
    }
}
