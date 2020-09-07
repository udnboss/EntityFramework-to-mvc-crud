using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WorkflowWeb.Business;
using WorkflowWeb.Models;
using WorkflowWeb.ViewModels;

namespace WorkflowWeb.Controllers
{
    public class BaseController<T, B, VM> : Controller where B : IBusiness<T>, new() where VM : BaseViewModel<T>, new() where T : new()
    {
        protected string user;
        protected IBusiness<T> business;
        protected DbContext db;

        public BaseController()
        {
            user = "Ali";
            db = new IMSEntities();
        }

        protected ContentResult JsonOut(object data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            return Content(json, "application/json");
        }

        object _routeFilter;
        protected T GetRouteFilter()
        {
            if (_routeFilter != null)
            {
                return (T)_routeFilter;
            }

            var ui_route_filter = (RouteData.Values["ui_route_filter"] ?? Request.QueryString["ui_route_filter"]) as string;
            if (!string.IsNullOrEmpty(ui_route_filter))
            {
                try
                {
                    var bytes = Convert.FromBase64String(ui_route_filter);
                    ui_route_filter = System.Text.Encoding.ASCII.GetString(bytes);

                    var filter = JsonConvert.DeserializeObject<VM>(ui_route_filter).ToModel(true);

                    _routeFilter = filter;

                    return filter;
                }
                catch
                {
                    return new T();
                }
            }

            return new T();
        }

        protected HttpStatusCode GetResponseCode<X>(BusinessResult<X> result)
        {
            switch (result.Status)
            {
                case State.AccessDenied:
                    return HttpStatusCode.Forbidden;
                case State.Error:
                    return HttpStatusCode.InternalServerError;
                case State.NoRecordFound:
                case State.NoRecordsAffected:
                    return HttpStatusCode.NotFound;
                case State.Success:
                    return HttpStatusCode.OK;
                default:
                    return HttpStatusCode.Conflict;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Log("OnActionExecuted", filterContext.RouteData);
            var routeFilter = GetRouteFilter();
            ViewBag.RouteFilter = routeFilter;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Log("OnActionExecuting", filterContext.RouteData);
            //var routeFilter = GetRouteFilter();
            //ViewBag.RouteFilter = routeFilter;
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Log("OnResultExecuted", filterContext.RouteData);
            //var routeFilter = GetRouteFilter();
            //ViewBag.RouteFilter = routeFilter;
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Log("OnResultExecuting ", filterContext.RouteData);
            //var routeFilter = GetRouteFilter();
            //ViewBag.RouteFilter = routeFilter;
        }

        private void Log(string methodName, RouteData routeData)
        {

        }

        //private ActionResult Index(object id = null)
        //{
        //    return View(id);
        //}

        //private ActionResult List(object id = null, string ui_list_view = null)
        //{
        //    ViewBag.CurrentID = id;
        //    var uiListView = ui_list_view ?? (RouteData.Values["ui_list_view"] ?? Request.QueryString["ui_list_view"]) as string;

        //    if (uiListView != null && uiListView != "ListDetail" && uiListView != "ListTable") //invalid
        //    {
        //        return HttpNotFound();
        //    }

        //    var results = business.GetList(GetRouteFilter());

        //    var message = results.Message;

        //    var responseCode = GetResponseCode(results);
        //    Response.StatusCode = (int)responseCode;

        //    if (responseCode == HttpStatusCode.OK)
        //    {
        //        var data = results.Data.Select(x => new VM().FromModel(x, true)).ToList().Cast<VM>().ToList();
        //        return PartialView(uiListView ?? "ListTable", data);
        //    }

        //    return Json(new string[] { message });
        //}

        //private ActionResult Details(object id)
        //{
        //    string message;

        //    if (id == null)
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        message = "Bad Request: missing identifier";
        //    }
        //    else
        //    {
        //        var r = business.Get(id);
        //        message = r.Message;

        //        var responseCode = GetResponseCode(r);
        //        Response.StatusCode = (int)responseCode;

        //        if (responseCode == HttpStatusCode.OK)
        //        {
        //            var m = r.Data;
        //            var vm = new VM().FromModel(m, true);
        //            return PartialView(vm);
        //        }
        //    }

        //    return Json(new string[] { message });
        //}

        //private ActionResult New()
        //{
        //    var routeFilter = GetRouteFilter();
        //    var vm = routeFilter != null ? new VM().FromModel(routeFilter, false) : new VM();
        //    var r = business.New(routeFilter);
        //    var message = r.Message;

        //    var responseCode = GetResponseCode(r);
        //    Response.StatusCode = (int)responseCode;

        //    if (responseCode == HttpStatusCode.OK)
        //    {
        //        ViewBag.Lookups = GetLookups();
        //        return PartialView(vm);
        //    }

        //    return Json(new string[] { message });
        //}

        //private ActionResult Edit(object id)
        //{
        //    string message;

        //    if (id == null)
        //    {
        //        Response.StatusCode = HttpStatusCode.BadRequest.GetHashCode();
        //        message = "Bad Request: missing identifier";
        //    }
        //    else
        //    {
        //        var r = business.Edit(id);
        //        message = r.Message;

        //        var responseCode = GetResponseCode(r);
        //        Response.StatusCode = (int)responseCode;
        //        if (responseCode == HttpStatusCode.OK)
        //        {
        //            var m = r.Data;
        //            var vm = new VM().FromModel(m, true);
        //            ViewBag.Lookups = GetLookups();
        //            return PartialView(vm);
        //        }
        //    }

        //    return Json(new string[] { message });
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IViewModel<T> vm)
        //{
        //    string message;

        //    if (ModelState.IsValid)
        //    {
        //        var m = vm.ToModel(false);
        //        m.GetType().GetProperty("ID").SetValue(m, Guid.NewGuid());
        //        //m.ID = Guid.NewGuid();
        //        var r = business.Insert(m);
        //        message = r.Message;

        //        var responseCode = GetResponseCode(r);
        //        Response.StatusCode = (int)responseCode;

        //        if (responseCode == HttpStatusCode.OK)
        //        {
        //            return List(m.GetType().GetProperty("ID").GetValue(m) as Guid?);
        //        }

        //        return Json(new string[] { message });
        //    }

        //    var errors = ModelState.SelectMany(x => x.Value.Errors)
        //        .Select(x => x.ErrorMessage)
        //        .ToList();

        //    Response.StatusCode = (int)HttpStatusCode.BadRequest;

        //    return Json(errors);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Update(IViewModel<T> vm)
        //{
        //    string message;

        //    if (ModelState.IsValid)
        //    {
        //        var m = vm.ToModel(false);
        //        var r = business.Update(m);
        //        message = r.Message;

        //        var responseCode = GetResponseCode(r);
        //        Response.StatusCode = (int)responseCode;
        //        if (responseCode == HttpStatusCode.OK)
        //        {
        //            return List(m.GetType().GetProperty("ID").GetValue(m) as Guid?);
        //        }

        //        return Json(new string[] { message });
        //    }

        //    var errors = ModelState.SelectMany(x => x.Value.Errors)
        //        .Select(x => x.ErrorMessage)
        //        .ToList();

        //    Response.StatusCode = (int)HttpStatusCode.BadRequest;

        //    return Json(errors);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(IViewModel<T> vm)
        //{
        //    string message;

        //    var m = vm.ToModel(false);
        //    var r = business.Delete(m);

        //    message = r.Message;
        //    var responseCode = GetResponseCode(r);
        //    Response.StatusCode = (int)responseCode;
        //    if (responseCode == HttpStatusCode.OK)
        //    {
        //        return List(null);
        //    }

        //    return Json(new string[] { message });
        //}


    }
}