
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
    public class _controller_Controller : BaseController
    {
        private _entities_ db = new _entities_();

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
            {
                return JsonOut(GetList());
            }
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

            return JsonOut(m);
        }

        public ActionResult New()
        {
            var vm = new
            {
                data = new _table_(),
                lookups = GetLookups()
            };

            return JsonOut(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(_table_ m)
        {
            if (ModelState.IsValid)
            {
                m.ID = _pktype_.New_pktype_(); db._table_.Add(m);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return JsonOut(ModelState);
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

            var vm = new
            {
                data = m,
                lookups = GetLookups()
            };

            return JsonOut(vm);
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

            return JsonOut(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(_table_ m)
        {
            return JsonOut(new { status = Del(m.ID) });
        }

    }
}
