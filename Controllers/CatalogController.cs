using Microsoft.AspNetCore.Mvc;
using ServiceCatalogAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ServiceCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/catalogtype")]
    public class CatalogController : ControllerBase
    {
        private readonly string _jsonPath = "catalogtypes.json";

        private List<Catalog> LoadCatalogs()
        {
            if (!System.IO.File.Exists(_jsonPath))
                return new List<Catalog>();
            var json = System.IO.File.ReadAllText(_jsonPath);
            var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("Catalogs").EnumerateArray()
                .Select(e => JsonSerializer.Deserialize<Catalog>(e.GetRawText()))
                .ToList();
            return items;
        }

        private void SaveCatalogs(List<Catalog> items)
        {
            var obj = new { Catalogs = items };
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_jsonPath, json);
        }

        [HttpPost("create")]
        public IActionResult CreateCatalog([FromBody] Catalog catalog)
        {
            var items = LoadCatalogs();
            // Always auto-generate CatalogId as CATXXXX
            int nextSeq = 1;
            if (items.Count > 0)
            {
                var last = items.OrderByDescending(i => i.CatalogId).FirstOrDefault();
                if (last != null && last.CatalogId != null && last.CatalogId.StartsWith("CAT"))
                {
                    var numPart = last.CatalogId.Substring(3);
                    if (int.TryParse(numPart, out int lastSeq))
                        nextSeq = lastSeq + 1;
                }
            }
            catalog.CatalogId = $"CAT{nextSeq.ToString("D4")}";
            items.Add(catalog);
            SaveCatalogs(items);
            return CreatedAtAction(nameof(ViewCatalog), new { id = catalog.CatalogId }, catalog);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateCatalog(string id, [FromBody] Catalog catalog)
        {
            var items = LoadCatalogs();
            var existing = items.FirstOrDefault(i => i.CatalogId == id);
            if (existing == null) return NotFound();
            existing.RequestType = catalog.RequestType;
            existing.RequestName = catalog.RequestName;
            existing.RequestDescription = catalog.RequestDescription;
            SaveCatalogs(items);
            return Ok(existing);
        }

        [HttpGet("view/{id}")]
        public IActionResult ViewCatalog(string id)
        {
            var items = LoadCatalogs();
            var item = items.FirstOrDefault(i => i.CatalogId == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("viewall")]
        public IActionResult ViewAllCatalogs()
        {
            var items = LoadCatalogs();
            return Ok(items);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteCatalog(string id)
        {
            var items = LoadCatalogs();
            var item = items.FirstOrDefault(i => i.CatalogId == id);
            if (item == null) return NotFound();
            items.Remove(item);
            SaveCatalogs(items);
            return NoContent();
        }

        [HttpDelete("deleteall")]
        public IActionResult DeleteAllCatalogs()
        {
            SaveCatalogs(new List<Catalog>());
            return NoContent();
        }
    }
}
