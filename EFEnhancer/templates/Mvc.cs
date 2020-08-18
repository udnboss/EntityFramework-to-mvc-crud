
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _namespace_.Models;

namespace _namespace_.Controllers
{
    public class _table_Controller : BaseController
    {
        public List<_table_> GetList()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db._table_.ToList();
            return data;
        }

        public _table_ Get(_pktype_ id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db._table_.Find(id);
        }

        public bool Del(_pktype_ id)
        {
            var m = Get(id);
            if (m == null)
            {
                return false;
            }

            db._table_.Remove(m);
            db.SaveChanges();
            return true;
        }

        public Dictionary<string, object> GetLookups()
        {
            return new Dictionary<string, object> {
                _lookups_
            };
        }

        public ActionResult Index()
        {
            return PartialView(GetList());            
        }

        public ActionResult Details(_pktype_ id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var m = Get(id);
            if (m == null)
            {
                return HttpNotFound();
            }

            return PartialView(m);
        }

        public ActionResult New()
        {
            var m = new _table_();
            ViewBag.Lookups = GetLookups();
            return PartialView(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(_table_ m)
        {
            if (ModelState.IsValid)
            {
                m.ID = _pktype_.New_pktype_(); 
                db._table_.Add(m);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public ActionResult Edit(_pktype_ id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var m = Get(id);
            if (m == null)
            {
                return HttpNotFound();
            }

            ViewBag.Lookups = GetLookups();
            return PartialView(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(_table_ m)
        {
            if (ModelState.IsValid)
            {
                db.Entry(m).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return PartialView(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(_table_ m)
        {
            Del(m.ID);
            return RedirectToAction("Index");
        }

    }
}
