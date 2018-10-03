using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Utbildning.Models;

namespace Utbildning.Controllers
{
    public class BookingDataController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BookingData
        public ActionResult Index()
        {
            return View(db.BookingDatas.ToList());
        }        
    }
}
