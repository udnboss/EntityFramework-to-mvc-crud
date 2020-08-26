
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
using _namespace_.Business;
using _namespace_.ViewModels;

namespace _namespace_.Controllers
{
    public partial class _table_Controller : BaseController<_table_, _table_Business, _table_ViewModel>
    {
        public _table_Controller()
        {
            business = new _table_Business(db, user);
            this.GetLookups = DefaultGetLookups;
        }

        public Func<Dictionary<string, object>> GetLookups { get; set; }

        public Dictionary<string, object> DefaultGetLookups()
        {
            var db = (_dbcontext_)this.db;
            var routeFilter = GetRouteFilter();

            return new Dictionary<string, object> {
                _lookups_
            };
        }

        public ActionResult Index(_nullablepktype_ id = null)
        {
            return View((object)id);
        }

        public ActionResult List(_nullablepktype_ id = null, string ui_list_view = null, bool json = false)
        {
            ViewBag.CurrentID = id;
            var uiListView = ui_list_view ?? (RouteData.Values["ui_list_view"] ?? Request.QueryString["ui_list_view"]) as string;

            if (uiListView != null && uiListView != "ListDetail" && uiListView != "ListTable") //invalid
            {
                return HttpNotFound();
            }

            var routeFilter = GetRouteFilter();
            var results = business.GetList(routeFilter);

            var message = results.Message;

            var responseCode = GetResponseCode(results);
            Response.StatusCode = (int)responseCode;

            if (responseCode == HttpStatusCode.OK)
            {
                var data = results.Data.Select(x => new _table_ViewModel(x, true)).ToList();
                if (json) { return JsonOut(data); }

                ViewBag.CanEdit = business.CanNew(routeFilter).Status == State.Success;

                return PartialView(uiListView ?? "ListTable", data);
            }

            return Json(new string[] { message });
        }

        public ActionResult DetailsWithBar(_pktype_ id, bool partial = true)
        {
            return Details(id, partial);
        }

        public ActionResult DetailsWithTabs(_pktype_ id, bool partial = true)
        {
            return Details(id, partial);

        }
        
        public ActionResult Details(_pktype_ id, bool partial = true, bool json = false)
        {
            string message;

            if (id == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                message = "Bad Request: missing identifier";
            }
            else
            {
                var r = business.Get(id);
                message = r.Message;

                var responseCode = GetResponseCode(r);
                Response.StatusCode = (int)responseCode;

                if (responseCode == HttpStatusCode.OK)
                {
                    var m = r.Data;
                    var vm = new _table_ViewModel(m, true);
                    if (json) { return JsonOut(vm); }

                    ViewBag.CanEdit = business.CanEdit(id).Status == State.Success;
                    ViewBag.CanDelete = business.CanDelete(m).Status == State.Success;

                    return partial ? PartialView(vm) as ActionResult : View(vm);
                }
            }

            return Json(new string[] { message });
        }

        public ActionResult Delete(_pktype_ id, bool json = false)
        {
            string message;

            if (id == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                message = "Bad Request: missing identifier";
            }
            else
            {
                var r = business.Get(id);
                message = r.Message;

                var responseCode = GetResponseCode(r);
                Response.StatusCode = (int)responseCode;

                if (responseCode == HttpStatusCode.OK)
                {
                    var m = r.Data;

                    var dr = business.CanDelete(m);
                    message = dr.Message;

                    responseCode = GetResponseCode(dr);
                    Response.StatusCode = (int)responseCode;

                    m = dr.Data;

                    var vm = new _table_ViewModel(m, true);
                    if (json) { return JsonOut(vm); }

                    return PartialView(vm);
                }
            }

            return Json(new string[] { message });
        }

        public ActionResult New(bool json = false)
        {
            var routeFilter = GetRouteFilter();
            var vm = routeFilter != null ? new _table_ViewModel(routeFilter) : new _table_ViewModel() { };
            var r = business.CanNew(routeFilter);
            var message = r.Message;

            var responseCode = GetResponseCode(r);
            Response.StatusCode = (int)responseCode;

            if (responseCode == HttpStatusCode.OK)
            {
                if (json) { return JsonOut(new { data = vm, lookups = GetLookups() }); }

                ViewBag.Lookups = GetLookups();                
                return PartialView(vm);
            }

            return Json(new string[] { message });
        }

        public ActionResult Edit(_pktype_ id, bool json = false)
        {
            string message;

            if (id == null)
            {
                Response.StatusCode = HttpStatusCode.BadRequest.GetHashCode();
                message = "Bad Request: missing identifier";
            }
            else
            {
                var r = business.CanEdit(id);
                message = r.Message;

                var responseCode = GetResponseCode(r);
                Response.StatusCode = (int)responseCode;
                if (responseCode == HttpStatusCode.OK)
                {
                    var m = r.Data;
                    var vm = new _table_ViewModel(m, true);
                                        
                    if (json) { return JsonOut(new { data = vm, lookups = GetLookups() }); }
                    ViewBag.Lookups = GetLookups();
                    return PartialView(vm);
                }
            }

            return Json(new string[] { message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(_table_ViewModel vm, bool json = false)
        {
            string message;

            if (ModelState.IsValid)
            {
                var m = vm.ToModel();
                _setnewguid_
                var r = business.Create(m);
                message = r.Message;

                var responseCode = GetResponseCode(r);
                Response.StatusCode = (int)responseCode;

                if (responseCode == HttpStatusCode.OK)
                {
                    return List(m.ID, null, json);
                }

                return Json(new string[] { message });
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(errors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(_table_ViewModel vm, bool json = false)
        {
            string message;

            if (ModelState.IsValid)
            {
                var m = vm.ToModel();
                var r = business.Update(m);
                message = r.Message;

                var responseCode = GetResponseCode(r);
                Response.StatusCode = (int)responseCode;
                if (responseCode == HttpStatusCode.OK)
                {
                    return List(m.ID, null, json);
                }

                return Json(new string[] { message });
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(errors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(_table_ViewModel vm, bool json = false)
        {
            string message;

            var m = vm.ToModel();
            var r = business.Delete(m);

            message = r.Message;
            var responseCode = GetResponseCode(r);
            Response.StatusCode = (int)responseCode;
            if (responseCode == HttpStatusCode.OK)
            {
                return List(null, null, json);
            }

            return Json(new string[] { message });
        }
    }
}
