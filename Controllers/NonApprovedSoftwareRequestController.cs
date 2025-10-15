using Microsoft.AspNetCore.Mvc;
using ServiceCatalogAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ServiceCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/nonapprovedsoftware")]
    public class NonApprovedSoftwareRequestController : ControllerBase
    {
        [HttpDelete("deleteall")]
        public IActionResult DeleteAllRequests()
        {
            SaveRequests(new List<NonApprovedSoftwareRequest>());
            return NoContent();
        }
        private readonly string _jsonPath = "nonapprovedsoftware.json";

        private List<NonApprovedSoftwareRequest> LoadRequests()
        {
            if (!System.IO.File.Exists(_jsonPath))
                return new List<NonApprovedSoftwareRequest>();
            var json = System.IO.File.ReadAllText(_jsonPath);
            var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("NonApprovedSoftwareRequests").EnumerateArray()
                .Select(e => JsonSerializer.Deserialize<NonApprovedSoftwareRequest>(e.GetRawText()))
                .ToList();
            return items;
        }

        private void SaveRequests(List<NonApprovedSoftwareRequest> items)
        {
            var obj = new { NonApprovedSoftwareRequests = items };
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_jsonPath, json);
        }

        [HttpPost("create")]
        public IActionResult CreateRequest([FromBody] NonApprovedSoftwareRequest request)
        {
            var items = LoadRequests();
            // Generate REQNAxxxx id
            int nextSeq = 1;
            if (items.Count > 0)
            {
                var last = items.OrderByDescending(i => i.Id).FirstOrDefault();
                if (last != null && last.Id != null && last.Id.StartsWith("REQNA"))
                {
                    var numPart = last.Id.Substring(5);
                    if (int.TryParse(numPart, out int lastSeq))
                        nextSeq = lastSeq + 1;
                }
            }
            request.Id = $"REQNA{nextSeq.ToString("D4")}";
            request.Status = "Pending";
            items.Add(request);
            SaveRequests(items);
            return CreatedAtAction(nameof(ViewRequest), new { id = request.Id }, request);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateRequest(string id, [FromBody] NonApprovedSoftwareRequest request)
        {
            var items = LoadRequests();
            var existing = items.FirstOrDefault(i => i.Id == id);
            if (existing == null) return NotFound();
            existing.RequestedFor = request.RequestedFor;
            existing.SoftwareName = request.SoftwareName;
            existing.SoftwareUrl = request.SoftwareUrl;
            existing.BusinessJustification = request.BusinessJustification;
            existing.NumberOfUsers = request.NumberOfUsers;
            existing.IsLicenseNeeded = request.IsLicenseNeeded;
            existing.Owner = request.Owner;
            if (!string.IsNullOrEmpty(request.Status))
            {
                existing.Status = request.Status;
            }
            SaveRequests(items);
            return Ok(existing);
        }

        [HttpGet("view/{id}")]
        public IActionResult ViewRequest(string id)
        {
            var items = LoadRequests();
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteRequest(string id)
        {
            var items = LoadRequests();
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();
            items.Remove(item);
            SaveRequests(items);
            return NoContent();
        }

        [HttpGet("viewall")]
        public IActionResult ViewAllRequests()
        {
            var items = LoadRequests();
            return Ok(items);
        }
    }
}