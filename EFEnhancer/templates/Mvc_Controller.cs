
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
            var data = db._table__include_.AsQueryable();

            var ui_route_filter = (RouteData.Values["ui_route_filter"] ?? Request.QueryString["ui_route_filter"]) as string;
            if (!string.IsNullOrEmpty(ui_route_filter))
            {
                try
                {
                    var bytes = Convert.FromBase64String(ui_route_filter);
                    ui_route_filter = System.Text.Encoding.ASCII.GetString(bytes);

                    var filter = JsonConvert.DeserializeObject<_table_ViewModel>(ui_route_filter).ToModel();

                    _filterconditions_                        
                }
                catch
                {

                }
            }

            return data.ToList().Select(x => new _table_ViewModel(x, true)).ToList();
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

        public ActionResult Index(_nullablepktype_ id = null)
        {
            return View(id);
        }

        public ActionResult ListDetail(_nullablepktype_ id = null)
        {
            ViewBag.CurrentID = id;
            return PartialView(GetList());
        }

        public ActionResult ListTable(_nullablepktype_ id = null)
        {
            ViewBag.CurrentID = id;
            return PartialView(GetList());
        }

        public ActionResult List(_nullablepktype_ id = null)
        {
            ViewBag.CurrentID = id;
            var ui_list_view = (RouteData.Values["ui_list_view"] ?? Request.QueryString["ui_list_view"]) as string;

            return PartialView(ui_list_view ?? "ListTableView", GetList());
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
                Response.StatusCode = HttpStatusCode.NotFound.GetHashCode();
                return Json(new string[] { "Item not found." });
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
                Response.StatusCode = HttpStatusCode.NotFound.GetHashCode();
                return Json(new string[] { "Item not found." });
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
                return List(m._pkname_);
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
                return List(m._pkname_);
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
                    Response.StatusCode = HttpStatusCode.NotFound.GetHashCode();
                    return Json(new string[] { "Item not found." });
                }

                db._table_.Remove(em);
                db.SaveChanges();

                return List(null);
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            Response.StatusCode = HttpStatusCode.BadRequest.GetHashCode();

            return Json(errors);
        }
    }
}
