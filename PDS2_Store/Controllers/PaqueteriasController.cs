﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PDS2_Store.Models;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using PDS2_Store.RepositorioDapper;

namespace PDS2_Store.Controllers
{
    [Authorize(Roles = "admin")]
    public class PaqueteriasController : Controller
    {
        private PaqueteriasContext db = new PaqueteriasContext();

        // GET: Paqueterias
        public async Task<ActionResult> Index()
        {
            return View(await db.Paqueterias.Include(x => x.pqt).Where(p => p.Activo == true).ToListAsync());
        }

        // GET: PaqueteriasParcial
        public async Task<ActionResult> ListaPaqueterias()
        {
            return View(await db.Paqueterias.Include(x => x.pqt).Where(p => p.Activo == true).ToListAsync());
        }

        // GET: Paqueterias/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paqueterias paqueterias = await db.Paqueterias.FindAsync(id);
            if (paqueterias == null)
            {
                return HttpNotFound();
            }
            return View(paqueterias);
        }

        // GET: Paqueterias/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Telefonos, "PaqueteriasId", "PaqueteriasId");
            ViewBag.Id = new SelectList(db.Correos, "PaqueteriasId", "PaqueteriasId");
            ViewBag.Id = new SelectList(db.Paquete, "PaqueteriasId", "PaqueteriasId");
            ViewBag.Destinos = db.Destinos.ToList();
            return View();
        }

        // POST: Paqueterias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Paqueterias paqueterias, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                string[] ids = fc.Get("Destinos").Trim().Split(',').ToArray();
                //string numero = fc["Destinos"];
                foreach (var item in ids)
                {
                    Debug.WriteLine("------->" + item);
                    int d1 = Convert.ToInt32(item);
                    Destinos n = await db.Destinos.FindAsync(d1);
                    paqueterias.des.Add(n);
                }
                paqueterias.Activo = true;
                db.Paqueterias.Add(paqueterias);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Paquete, "PaqueteriasId", "PaqueteriasId", paqueterias.Id);
            ViewBag.Id = new SelectList(db.Telefonos, "PaqueteriasId", "PaqueteriasId", paqueterias.Id);
            ViewBag.Id = new SelectList(db.Correos, "PaqueteriasId", "PaqueteriasId", paqueterias.Id);
            return View(paqueterias);
        }

        // GET: Paqueterias/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paqueterias paqueterias = await db.Paqueterias.FindAsync(id);
            if (paqueterias == null)
            {
                return HttpNotFound();
            }
            return View(paqueterias);
        }

        // POST: Paqueterias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Paqueterias paqueterias)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paqueterias).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(paqueterias);
        }

        // GET: Paqueterias/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paqueterias paqueterias = await db.Paqueterias.FindAsync(id);
            if (paqueterias == null)
            {
                return HttpNotFound();
            }
            return View(paqueterias);
        }

        // POST: Paqueterias/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {

            var paq = new Paqueterias() { Id = id, Activo = false};
            using(db)
            {
                db.Paqueterias.Attach(paq);
                db.Entry(paq).Property(p => p.Activo).IsModified = true;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
