using Microsoft.AspNetCore.Mvc;
using ServiceCatalogAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ServiceCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/applicationservicerequest")]
    public class ApplicationServiceRequestController : ControllerBase
    {
        private readonly string _jsonPath = "applicationservicerequests.json";

        private List<ApplicationServiceRequest> LoadItems()
        {
            if (!System.IO.File.Exists(_jsonPath))
                return new List<ApplicationServiceRequest>();
            var json = System.IO.File.ReadAllText(_jsonPath);
            var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("ApplicationServiceRequests").EnumerateArray()
                .Select(e => JsonSerializer.Deserialize<ApplicationServiceRequest>(e.GetRawText()))
                .ToList();
            return items;
        }

        private void SaveItems(List<ApplicationServiceRequest> items)
        {
            var obj = new { ApplicationServiceRequests = items };
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_jsonPath, json);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] ApplicationServiceRequest item)
        {
            var items = LoadItems();
            // Generate REQARXXXX id
            int nextSeq = 1;
            if (items.Count > 0)
            {
                var last = items.OrderByDescending(i => i.Id).FirstOrDefault();
                if (last != null && last.Id != null && last.Id.StartsWith("REQAR"))
                {
                    var numPart = last.Id.Substring(5);
                    if (int.TryParse(numPart, out int lastSeq))
                        nextSeq = lastSeq + 1;
                }
            }
            item.Id = $"REQAR{nextSeq.ToString("D4")}";
            items.Add(item);
            SaveItems(items);
            return CreatedAtAction(nameof(View), new { id = item.Id }, item);
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(string id, [FromBody] ApplicationServiceRequest item)
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
            SaveItems(new List<ApplicationServiceRequest>());
            return NoContent();
        }
    }
}
