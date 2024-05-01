using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class Actor
    {
        public int ActorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    public class ActorsController : ApiController
    {
        private static readonly List<Actor> _actors = new List<Actor>
        {
            new Actor
            {
                ActorID = 1,
                FirstName = "Tom",
                LastName = "Hanks",
                DateOfBirth = new DateTime(1956, 7, 9)
            },
            new Actor
            {
                ActorID = 2,
                FirstName = "Brad",
                LastName = "Pitt",
                DateOfBirth = new DateTime(1963, 12, 18)
            },
            new Actor
            {
                ActorID = 3,
                FirstName = "Meryl",
                LastName = "Streep",
                DateOfBirth = new DateTime(1949, 6, 22)
            },
        };

        [System.Web.Http.Route("api/actors/all")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GelAllActor()
        {
            return Ok(_actors);
        }

        [System.Web.Http.Route("api/actors/add")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult AddActor([FromBody] Actor actor)
        {
            if (actor == null)
            {
                return BadRequest("Invalid actor data");
            }

            actor.ActorID = _actors.Count + 1;


            _actors.Add(actor);


            return Ok(actor);
        }

        [System.Web.Http.Route("api/actors/update/{id}")]
        [System.Web.Http.HttpPut]
        public IHttpActionResult Put(int id, [FromBody] Actor actor)
        {
            if (actor == null)
            {
                return BadRequest("Invalid actor data");
            }

            Actor existingActor = _actors.FirstOrDefault(a => a.ActorID == id);
            if (existingActor == null)
            {
                return NotFound();
            }

            existingActor.FirstName = actor.FirstName;
            existingActor.LastName = actor.LastName;
            existingActor.DateOfBirth = actor.DateOfBirth;

            return Ok(existingActor);
        }


        [System.Web.Http.Route("api/actors/delete/{id}")]
        [System.Web.Http.HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            Actor actorToDelete = _actors.FirstOrDefault(a => a.ActorID == id);
            if (actorToDelete == null)
            {
                return NotFound();
            }
            _actors.Remove(actorToDelete);

            return Ok(actorToDelete);
        }

        [System.Web.Http.Route("api/actors/search")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult SearchActors(string firstName = null)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                return BadRequest("First name is required for searching actors.");
            }

            IEnumerable<Actor> result = _actors.Where(a => a.FirstName.ToLower().Contains(firstName.ToLower()));

            return Ok(result);
        }

        [System.Web.Http.Route("api/actors/filter")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult FilterActors(string firstName = null, string lastName = null)
        {
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                return BadRequest("At least one parameter (firstName or lastName) is required for filtering actors.");
            }

            IEnumerable<Actor> result = _actors;

            if (!string.IsNullOrEmpty(firstName))
            {
                result = result.Where(a => a.FirstName.ToLower().Contains(firstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                result = result.Where(a => a.LastName.ToLower().Contains(lastName.ToLower()));
            }

            return Ok(result);
        }
    }
}