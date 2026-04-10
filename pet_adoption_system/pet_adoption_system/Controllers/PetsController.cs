//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using pet_adoption_system.Models;

//namespace pet_adoption_system.Controllers
//{
//    public class PetsController : Controller
//    {
//        private PetAdoptionDbContext db = new PetAdoptionDbContext();

//        // GET: Pets
//        public ActionResult Index()
//        {
//            return View(db.Pets.ToList());
//        }

//        // GET: Pets/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Pet pet = db.Pets.Find(id);
//            if (pet == null)
//            {
//                return HttpNotFound();
//            }
//            return View(pet);
//        }

//        // GET: Pets/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        // POST: Pets/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "PetId,PetName,AdoptionFee")] Pet pet)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Pets.Add(pet);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            return View(pet);
//        }

//        // GET: Pets/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Pet pet = db.Pets.Find(id);
//            if (pet == null)
//            {
//                return HttpNotFound();
//            }
//            return View(pet);
//        }

//        // POST: Pets/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "PetId,PetName,AdoptionFee")] Pet pet)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(pet).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            return View(pet);
//        }

//        // GET: Pets/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Pet pet = db.Pets.Find(id);
//            if (pet == null)
//            {
//                return HttpNotFound();
//            }
//            return View(pet);
//        }

//        // POST: Pets/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            Pet pet = db.Pets.Find(id);
//            db.Pets.Remove(pet);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using pet_adoption_system.Models;

namespace pet_adoption_system.Controllers
{
    [Authorize]
    public class PetsController : Controller
    {
        private PetAdoptionDbContext db = new PetAdoptionDbContext();

        public ActionResult Index()
        {
            return View(db.Pets.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PetId,PetName,AdoptionFee")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                db.Pets.Add(pet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pet);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PetId,PetName,AdoptionFee")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pet);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Pet pet = db.Pets.Find(id);
                if (pet != null)
                {
                    // Step 1: Remove related entries from AdoptionEntries table first
                    var relatedEntries = db.AdoptionEntries.Where(ae => ae.PetId == id).ToList();
                    db.AdoptionEntries.RemoveRange(relatedEntries);

                    // Step 2: Now safe to remove the pet from Pets table
                    db.Pets.Remove(pet);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Return to view with error if something goes wrong
                ModelState.AddModelError("", "Unable to delete: " + ex.Message);
                return View(db.Pets.Find(id));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}