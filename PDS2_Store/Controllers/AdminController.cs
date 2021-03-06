﻿using PDS2_Store.Models;
using PDS2_Store.RepositorioDapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PDS2_Store.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/GetRequests
        // Esto regrea una lista de las solicitudes para ser vendedor
        public ActionResult GetRequests()
        {
            RepoDapper Repo = new RepoDapper();
            List<RequestViewModel> requestViewModel = Repo.GetRequests();
            return View(requestViewModel);
        }

        public ActionResult Usuarios()
        {
            var context = new IdentityDbContext();
            var usersWithRoles = (from user in context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Phone = user.PhoneNumber,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new UsersViewModel()

                                  {
                                      Userid = p.UserId,
                                      UserName = p.Username,
                                      PhoneNumber = p.Phone,
                                      Role = string.Join(",", p.RoleNames)
                                  });
            return View(usersWithRoles);
        }


        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/EditRequests/5
        public ActionResult EditRequests(int id)
        {
            List<Statu> StatuList = new List<Statu>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                StatuList = db.Query<Statu>("Select * From Statu").ToList();
            }
            ViewBag.Statu = new SelectList(StatuList, "Id", "Stat");
            var userId = User.Identity.GetUserId();
            RepoDapper DirRepo = new RepoDapper();
            return View(DirRepo.GetRequests().Find(Dir => Dir.id == id));
        }

        // POST: Admin/EditRequests/5
        [HttpPost]
        public ActionResult EditRequests(int id, int Statu)
        {
             try
            {
                Request requestUser= new Request();
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    requestUser = db.Query<Request>("Select * From Request WHERE id=" + id, new { id }).SingleOrDefault();
                }

                RequestStatus statu = new RequestStatus();
                statu.RequestId = id;
                statu.StatusId = Statu;
                RepoDapper Repo = new RepoDapper();
                Repo.Cambio_de_estado(statu);
               
                if(statu.StatusId == 2)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                        usermanager.AddToRole(requestUser.UserId, "vendedor");
                        context.SaveChanges();
                    }
                }
                else
                {
                    if(statu.StatusId== 4)
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                            usermanager.RemoveFromRole(requestUser.UserId, "vendedor");
                            context.SaveChanges();
                        }
                    }
                }
           

                return RedirectToAction("GetRequests");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
