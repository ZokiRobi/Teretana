using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Teretana.Models;

namespace Teretana.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index(string sortBy = null, string searchTerm = null, int page = 1, bool Searched = false)
        {
            if (searchTerm != null)
            {
                var s = searchTerm.Split(' ');
                searchTerm = s[0];
            }
            using (TeretanaEntities db = new TeretanaEntities())
            {
                IEnumerable<Clanovi> model = db.Clanovi
                              .Where(x => searchTerm == null
                              || x.Ime.StartsWith(searchTerm)
                              || x.Prezime.StartsWith(searchTerm));

                if (model.Count() < 5 || Searched)
                    page = 1;


                switch (sortBy)
                {
                    case "name":
                        model = model.OrderBy(x => x.Ime).ToPagedList(page, 5);
                        break;
                    case "Prezime":
                        model = model.OrderBy(x => x.Prezime).ToPagedList(page, 5);
                        break;
                    case "VremeOd":
                        model = model.OrderBy(x => x.VremeOd).ToPagedList(page, 5);
                        break;
                    case "VremeDo":
                        model = model.OrderBy(x => x.VremeDo).ToPagedList(page, 5);
                        break;
                    default:
                        model = model.OrderBy(x => x.ClanoviId).ToPagedList(page, 5);
                        break;
                }

                ViewBag.searchTerm = searchTerm;
                ViewBag.page = page;


                return View(model);
            }

        }

        public ActionResult AutoComplete(string term)
        {
            TeretanaEntities db = new TeretanaEntities();

            var model = db.Clanovi.Where(x => x.Ime.StartsWith(term) || x.Prezime.StartsWith(term)).Take(10).Select(x => new { label = x.Ime + " " + x.Prezime });
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DodajClana()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DodajClana(Clanovi clan)
        {

            if (ModelState.IsValid)
            {
                using (TeretanaEntities db = new TeretanaEntities())
                {
                    if (db.Clanovi.Any(x => x.Ime == clan.Ime && x.Prezime == clan.Prezime) && clan.ClanoviId == 0)
                    {
                        ModelState.AddModelError("", "Vec postoji clan sa tim Imenom i Prezimenom");
                        return View(clan);
                    }
                    else
                    {
                        var dbClan = db.Clanovi.Find(clan.ClanoviId);
                        if (dbClan == null && !db.Clanovi.Any(x => x.Ime == clan.Ime && x.Prezime == clan.Prezime))
                        {
                            db.Clanovi.Add(clan);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            dbClan.Ime = clan.Ime;
                            dbClan.Prezime = clan.Prezime;
                            dbClan.VremeDo = clan.VremeDo;
                            dbClan.VremeOd = clan.VremeOd;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }

                    }
                }
            }
            return View(clan);
        }
        public ActionResult Edit(int id)
        {
            using (TeretanaEntities db = new TeretanaEntities())
            {
                var model = db.Clanovi.Find(id);
                return View("DodajClana", model);
            }
        }
        public ActionResult Delete(int id)
        {
            using (TeretanaEntities db = new TeretanaEntities())
            {
                var clan = db.Clanovi.Find(id);
                if (clan != null)
                {
                    db.Clanovi.Remove(clan);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

        public ActionResult PreostaloVreme(int id)
        {
            using (TeretanaEntities db = new TeretanaEntities())
            {
                var clan = db.Clanovi.Find(id);
                var vreme = clan.VremeDo.Subtract(clan.VremeOd).TotalDays;
                return View(vreme);
            }
        }

    }
}