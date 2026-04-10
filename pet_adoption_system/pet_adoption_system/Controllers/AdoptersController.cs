using pet_adoption_system.Models;
using pet_adoption_system.Models.View_Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity; 

namespace pet_adoption_system.Controllers
{
    [Authorize]
    public class AdoptersController : Controller
    {
        
        private readonly PetAdoptionDbContext db = new PetAdoptionDbContext();
        // GET: Adopters
        public ActionResult Index()
        {
            var adopters = db.Adopters
                      .Include(x => x.AdoptionEntries.Select(ae => ae.Pet))
                      .OrderByDescending(x => x.AdopterId)
                      .ToList();
            return View(adopters);
        }
        public ActionResult AddNewPet(int?id)
        {
            ViewBag.pets = new SelectList(db.Pets.ToList(), "PetId", "PetName", (id != null) ? id.ToString() : "");
            return PartialView("_AddNewPet");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(AdopterVM adopterVM, int[] PetId)
        {
            if (PetId == null || !PetId.Any(p => p > 0))
            {
                return PartialView("_error");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Adopter adopter = new Adopter()
                    {
                        AdopterName = adopterVM.AdopterName,
                        BirthDate = adopterVM.BirthDate,
                        Age = adopterVM.Age,
                        Maritalstatus = adopterVM.Maritalstatus
                    };

                    HttpPostedFileBase file = adopterVM.PictureFile;
                    if (file != null)
                    {
                        string extension = Path.GetExtension(file.FileName);
                        string fileName = "/Images/" + DateTime.Now.Ticks.ToString() + extension;
                        file.SaveAs(Server.MapPath(fileName));
                        adopter.Picture = fileName;
                    }

                    db.Adopters.Add(adopter);
                    db.SaveChanges();

                    foreach (var item in PetId)
                    {
                        if (item > 0)
                        {
                            AdoptionEntry adoptionentry = new AdoptionEntry()
                            {
                                AdopterId = adopter.AdopterId,
                                PetId = item
                            };
                            db.AdoptionEntries.Add(adoptionentry);
                        }
                    }
                    db.SaveChanges();

                    return PartialView("_success");
                }
                catch (Exception)
                {
                    return PartialView("_error");
                }
            }

            return PartialView("_error");
        }
        public ActionResult Edit(int? id)
        {
            Adopter adopter = db.Adopters.First(x => x.AdopterId == id);
            var adoptionEntry = db.AdoptionEntries.Where(x => x.AdopterId == id).ToList();
            AdopterVM adopterVM = new AdopterVM()
            {
                AdopterId = adopter.AdopterId,
                AdopterName = adopter.AdopterName,
                BirthDate = adopter.BirthDate,
                Age = adopter.Age,
                Picture = adopter.Picture,
                Maritalstatus = adopter.Maritalstatus
            };
            if (adoptionEntry.Count() > 0)
            {
                foreach (var item in adoptionEntry)
                {
                    adopterVM.PetList.Add(item.PetId);
                }
            }
            return View(adopterVM);
        }
        [HttpPost]
        public ActionResult Edit(AdopterVM adopterVM, int[] petId)
        {
            if (ModelState.IsValid)
            {
                var adopterInDb = db.Adopters.Find(adopterVM.AdopterId);

                if (adopterInDb == null)
                {
                    return HttpNotFound();
                }

                adopterInDb.AdopterName = adopterVM.AdopterName;
                adopterInDb.BirthDate = adopterVM.BirthDate;
                adopterInDb.Age = adopterVM.Age;
                adopterInDb.Maritalstatus = adopterVM.Maritalstatus;

                if (adopterVM.PictureFile != null)
                {
                    string fileName = Path.Combine("/Images/", DateTime.Now.Ticks.ToString() + Path.GetExtension(adopterVM.PictureFile.FileName));
                    adopterVM.PictureFile.SaveAs(Server.MapPath(fileName));
                    adopterInDb.Picture = fileName;
                }

                var existingEntries = db.AdoptionEntries.Where(x => x.AdopterId == adopterVM.AdopterId).ToList();
                db.AdoptionEntries.RemoveRange(existingEntries);

                if (petId != null)
                {
                    foreach (var pId in petId)
                    {
                        db.AdoptionEntries.Add(new AdoptionEntry
                        {
                            AdopterId = adopterVM.AdopterId,
                            PetId = pId
                        });
                    }
                }

                db.SaveChanges();
                return PartialView("_success");
            }
            return PartialView("_error");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Adopter adopter = db.Adopters.FirstOrDefault(x => x.AdopterId == id);

            if (adopter == null)
            {
                return HttpNotFound();
            }

            var adoptionEntry = db.AdoptionEntries.Where(x => x.AdopterId == id).ToList();

            AdopterVM adopterVM = new AdopterVM()
            {
                AdopterId = adopter.AdopterId,
                AdopterName = adopter.AdopterName,
                BirthDate = adopter.BirthDate,
                Age = adopter.Age,
                Picture = adopter.Picture,
                Maritalstatus = adopter.Maritalstatus,
                PetList = new List<int>()
            };

            if (adoptionEntry.Count() > 0)
            {
                foreach (var item in adoptionEntry)
                {
                    adopterVM.PetList.Add(item.PetId);
                }
            }

            return View(adopterVM);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Adopter adopter = db.Adopters.Find(id);

            if (adopter == null)
            {
                return HttpNotFound();
            }

            var petEntry = db.AdoptionEntries.Where(x => x.AdopterId == adopter.AdopterId).ToList();
            db.AdoptionEntries.RemoveRange(petEntry);

            db.Entry(adopter).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}