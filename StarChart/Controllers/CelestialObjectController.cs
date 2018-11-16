using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var model = _context.CelestialObjects.Find(id);
            if (model == null)
                return NotFound();

            model.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            return Ok(model);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var model = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (!model.Any())
                return NotFound();

            foreach (var item in model)
            {
                item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();
            }
            return Ok(model);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var record = _context.CelestialObjects.ToList();
            if (!record.Any())
                return NotFound();
            foreach (var item in record)
            {
                item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();
            }
            return Ok(record);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var record = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (record == null)
                return NotFound();

            record.Name = celestialObject.Name;
            record.OrbitalPeriod = celestialObject.OrbitalPeriod;
            record.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(record);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var record = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (record == null)
                return NotFound();
            record.Name = name;
            _context.CelestialObjects.Update(record);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var records = _context.CelestialObjects.Where( x=> x.Id == id || x.OrbitedObjectId == id);
            if(!records.Any())
                return NotFound();
            _context.CelestialObjects.RemoveRange(records);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
