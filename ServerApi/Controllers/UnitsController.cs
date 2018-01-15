using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Models.EFDB;
using ServerApi.Services;

namespace ServerApi.Controllers
{
    [RoutePrefix("api/units")]
    public class UnitsController : ApiController
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        [Route("")]
        [HttpGet]
        [ResponseType(typeof(List<Unit>))]
        public async Task<IHttpActionResult> GetUnitsAsync()
        {
            return Ok(await _db.Units.ToListAsync());
        }

        [Route("{id}")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetUnitAsync(int id)
        {
            Unit unit = _db.Units.Find(id);
            if (unit == null)
            {
                return NotFound();
            }

            return Ok(unit);
        }

        private bool UnitExists(int id)
        {
            return _db.Units.Count(e => e.Id == id) > 0;
        }
    }
}