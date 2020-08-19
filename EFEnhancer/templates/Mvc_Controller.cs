
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
using _namespace_.ViewModels;

namespace _namespace_.Controllers
{
    public class _table_Controller : BaseController
    {
        public List<_table_ViewModel> GetList()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db._table__include_.ToList().Select(x => new _table_ViewModel(x, true)).ToList();
            return data;
        }

        public _table_ Get(_pktype_ id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db._table__include_.FirstOrDefault(x=> x._pkname_ == id);
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

            var vm = new _table_ViewModel(m, true);

            return PartialView(vm);
        }

        public ActionResult New()
        {
            var vm = new _table_ViewModel() { _defaults_ };
                       
            ViewBag.Lookups = GetLookups();
            return PartialView(vm);
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

            var vm = new _table_ViewModel(m);
            ViewBag.Lookups = GetLookups();

            return PartialView(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(_table_ViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var m = vm.ToModel();
                _setnewguid_
                db._table_.Add(m);
                db.SaveChanges();
                return PartialView("Index", GetList());
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            Response.StatusCode = HttpStatusCode.BadRequest.GetHashCode();

            return Json(errors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(_table_ViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var m = vm.ToModel();
                db.Entry(m).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("Index", GetList());
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            Response.StatusCode = HttpStatusCode.BadRequest.GetHashCode();

            return Json(errors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(_table_ViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var em = Get(vm.ID);
                if (em == null)
                {
                    return HttpNotFound();
                }

                db._table_.Remove(em);
                db.SaveChanges();

                return PartialView("Index", GetList());
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            Response.StatusCode = HttpStatusCode.BadRequest.GetHashCode();

            return Json(errors);
        }
    }
}
