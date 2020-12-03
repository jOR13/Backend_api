using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using API_BC.Models;
using System.IO;
using System.Web;

namespace API_BC.Controllers
{
    public class usersController : ApiController
    {
        private bcEntities db = new bcEntities();

        // GET: api/users
        public object Getusers()
        {
            return Json(db.users);
        }

        // GET: api/users/5
        [ResponseType(typeof(user))]
        public IHttpActionResult Getuser(int id)
        {
            user user = db.users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        //[Route("api/users/login")]
        //public bool postuserlog(user user)
        //{
        //    bool logueado = false;
        //    user = db.users.where(u => u.email.equals(user.email) && u.password.equals(user.password)).firstordefault();



        //    if (user != null)
        //    {
        //        logueado = true;
        //    }
        //    return logueado;
        //}


        [Route("api/users/login")]
        public object PostuserLog(user user)
        {
            bool logueado = false;
            user = db.users.Where(u => u.email.Equals(user.email) && u.password.Equals(user.password)).FirstOrDefault();



            if (user != null)
            {
                logueado = true;
            }

            user.status = true;

            return Json(user);
        }

        // PUT: api/users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putuser(int id, user user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/users
        [ResponseType(typeof(user))]
        public IHttpActionResult Postuser(user user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string imageBase64 = user.picture.ToString();

            HttpContext context = HttpContext.Current;
            string localPath = context.Server.MapPath(@"\Content\Images\");

            File.WriteAllBytes(localPath+user.nombre+"-"+user.email+".png", Convert.FromBase64String(imageBase64));

            user.picture = user.nombre + "-" + user.email + ".png";

            db.users.Add(user);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (userExists(user.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = user.id }, user);
        }

        // DELETE: api/users/5
        [ResponseType(typeof(user))]
        public IHttpActionResult Deleteuser(int id)
        {
            user user = db.users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private bool userExists(int id)
        {
            return db.users.Count(e => e.id == id) > 0;
        }
    }
}