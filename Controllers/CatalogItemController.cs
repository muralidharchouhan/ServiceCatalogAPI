using Microsoft.AspNetCore.Mvc;
using ServiceCatalogAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ServiceCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/catalogitem")]
    public class CatalogItemController : ControllerBase
    {
        private readonly string _jsonPath = "catalog.json";

        private List<CatalogItem> LoadItems()
        {
            if (!System.IO.File.Exists(_jsonPath))
                return new List<CatalogItem>();
            var json = System.IO.File.ReadAllText(_jsonPath);
            var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("CatalogItems").EnumerateArray()
                .Select(e => JsonSerializer.Deserialize<CatalogItem>(e.GetRawText()))
                .ToList();
            return items;
        }

        private void SaveItems(List<CatalogItem> items)
        {
            var obj = new { CatalogItems = items };
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_jsonPath, json);
        }

        [HttpPost("create")]
        public IActionResult CreateCatalogItem([FromBody] CatalogItem item)
        {
            var items = LoadItems();
            // Generate REQXXXX id
            int nextSeq = 1;
            if (items.Count > 0)
            {
                var last = items.OrderByDescending(i => i.Id).FirstOrDefault();
                if (last != null && last.Id != null && last.Id.StartsWith("REQ"))
                {
                    var numPart = last.Id.Substring(3);
                    if (int.TryParse(numPart, out int lastSeq))
                        nextSeq = lastSeq + 1;
                }
            }
            item.Id = $"REQ{nextSeq.ToString("D4")}";
            items.Add(item);
            SaveItems(items);
            return CreatedAtAction(nameof(ViewCatalogItem), new { id = item.Id }, item);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateCatalogItem(string id, [FromBody] CatalogItem item)
        {
            var items = LoadItems();
            var existing = items.FirstOrDefault(i => i.Id == id);
            if (existing == null) return NotFound();
            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.Category = item.Category;
            existing.AssignedTo = item.AssignedTo;
            existing.RequestedFor = item.RequestedFor;
            existing.SupportGroup = item.SupportGroup;
            SaveItems(items);
            return Ok(existing);
        }
        [HttpGet("viewall")]
        public IActionResult ViewAllCatalogItems()
        {
            var items = LoadItems();
            return Ok(items);
        }

        [HttpGet("view/{id}")]
        public IActionResult ViewCatalogItem(string id)
        {
            var items = LoadItems();
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteCatalogItem(string id)
        {
            var items = LoadItems();
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();
            items.Remove(item);
            SaveItems(items);
            return NoContent();
        }
    }
}
