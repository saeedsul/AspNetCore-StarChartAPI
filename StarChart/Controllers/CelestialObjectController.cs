using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
            var model = _context.CelestialObjects.Where(x=>x.Name == name).ToList();
            if(!model.Any())
                return NotFound();

            foreach(var item in model)
            {
                item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();                
            }            
            return Ok(model);
        }

        public IActionResult GetAll()
        {
            var model = _context.CelestialObjects.ToList();
            if(!model.Any())
                return NotFound();
            foreach(var item in model)
            {
                item.Satellites = _context.CelestialObjects.Where(x=>x.OrbitedObjectId == item.Id).ToList();
            }
            return Ok(model);
        }
    }
}
