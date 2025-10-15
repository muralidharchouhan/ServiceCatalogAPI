using Microsoft.AspNetCore.Mvc;
using ServiceCatalogAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ServiceCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/aiintake")]
    public class AIIntakeRequestController : ControllerBase
    {
        private readonly string _jsonPath = "aiintakerequests.json";

        private List<AIIntakeRequest> LoadItems()
        {
            if (!System.IO.File.Exists(_jsonPath))
                return new List<AIIntakeRequest>();
            var json = System.IO.File.ReadAllText(_jsonPath);
            var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("AIIntakeRequests").EnumerateArray()
                .Select(e => JsonSerializer.Deserialize<AIIntakeRequest>(e.GetRawText()))
                .ToList();
            return items;
        }

        private void SaveItems(List<AIIntakeRequest> items)
        {
            var obj = new { AIIntakeRequests = items };
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_jsonPath, json);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] AIIntakeRequest item)
        {
            var items = LoadItems();
            // Generate REQAIXXXX id
            int nextSeq = 1;
            if (items.Count > 0)
            {
                var last = items.OrderByDescending(i => i.Id).FirstOrDefault();
                if (last != null && last.Id != null && last.Id.StartsWith("REQAI"))
                {
                    var numPart = last.Id.Substring(5);
                    if (int.TryParse(numPart, out int lastSeq))
                        nextSeq = lastSeq + 1;
                }
            }
            item.Id = $"REQAI{nextSeq.ToString("D4")}";
            items.Add(item);
            SaveItems(items);
            return CreatedAtAction(nameof(View), new { id = item.Id }, item);
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(string id, [FromBody] AIIntakeRequest item)
        {
            var items = LoadItems();
            var existing = items.FirstOrDefault(i => i.Id == id);
            if (existing == null) return NotFound();
            existing.AIProductNames = item.AIProductNames;
            existing.UseCaseDescription = item.UseCaseDescription;
            existing.VendorName = item.VendorName;
            SaveItems(items);
            return Ok(existing);
        }

        [HttpGet("view/{id}")]
        public IActionResult View(string id)
        {
            var items = LoadItems();
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("viewall")]
        public IActionResult ViewAll()
        {
            var items = LoadItems();
            return Ok(items);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(string id)
        {
            var items = LoadItems();
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();
            items.Remove(item);
            SaveItems(items);
            return NoContent();
        }

        [HttpDelete("deleteall")]
        public IActionResult DeleteAll()
        {
            SaveItems(new List<AIIntakeRequest>());
            return NoContent();
        }
    }
}
