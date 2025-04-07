using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            var insurees = db.Insurees.AsNoTracking().ToList();
            return View(insurees);
        }


        public ActionResult Admin()
        {
            var insurees = db.Insurees.ToList();
            return View(insurees);
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Email,DateOfBirth,CarYear,CarMake,CarModel,SpeedingTickets,DUI,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                decimal quote = 50;
                int age = 0;
                if (insuree.DateOfBirth.HasValue)
                {
                    age = DateTime.Now.Year - insuree.DateOfBirth.Value.Year;
                }

                if (age <= 18) quote += 100;
                else if (age >= 19 && age <= 25) quote += 50;
                else quote += 25;

                if(insuree.CarYear < 2000)
                {
                    quote = quote + 25;
                }

                if (insuree.CarYear > 2015)
                {
                    quote = quote + 25;
                }

                if (insuree.CarMake.ToLower() == "porsche") quote += 25;
                if (insuree.CarMake.ToLower() == "porsche" && insuree.CarModel.ToLower() == "911 carrera") quote += 25;

                if (insuree.SpeedingTickets.HasValue) {
                    quote = quote + (insuree.SpeedingTickets.Value * 10);
                }

                if (insuree.DUI.HasValue && insuree.DUI.Value) quote *= 1.25m;

                if (insuree.CoverageType.HasValue && insuree.CoverageType.Value) quote *= 1.50m;

                insuree.Quote = quote;
                
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,DateOfBirth,CarYear,CarMake,CarModel,SpeedingTickets,DUI,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                Debug.WriteLine("Inside state updation : " +insuree.DateOfBirth);
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                db.Entry(insuree).Reload();
                ModelState.Clear();
                return RedirectToAction("Index");
            }
            Debug.WriteLine("After state updation");
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
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
