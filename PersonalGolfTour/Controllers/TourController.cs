using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalGolfTour.Data;
using PersonalGolfTour.Models;
using PersonalGolfTour.Models.TourViewModels;

namespace PersonalGolfTour.Controllers
{
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TourController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tour
        public async Task<IActionResult> Index()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string id = _userManager.GetUserId(User);
            var query = from tour in _context.Tours
                        where tour.UserTours.Any(ut => ut.UserId.Equals(id))
                        select tour;
            return View(query);
        }

        // GET: Tour/TourStandings/5
        public async Task<IActionResult> TourStandings(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("ChosenTourId", (int)id);

            // New Code, build a TourViewModel with necessary components
            TourViewModel tvm = new TourViewModel();

            var t = (from tour in _context.Tours
                      where tour.TourId == id
                      select tour).FirstOrDefault();
            tvm.Tour = t;

            
            var players = (from player in _context.Users
                             where player.UserTours.Any(ut => ut.UserId.Equals(player.Id) && ut.TourId == id)
                             select player).ToList();
            var placementrulesquery = (from placementrule in _context.PlacementRules
                                 where placementrule.TourId == tvm.Tour.TourId
                                 select placementrule).ToList();
            var standings = new Dictionary<ApplicationUser, StandingsViewModel>();
            var placementmap = new Dictionary<int, PlacementRule>();

            foreach (var player in players)
            {
                standings.Add(player, new StandingsViewModel { Score = 0, Player = player });
            }
            foreach (var placementrule in placementrulesquery)
            {
                placementmap.Add(placementrule.Place, placementrule);
            }

            var tourevents = (from tourevent in _context.TourEvents
                              where tourevent.TourId == tvm.Tour.TourId
                              select tourevent).ToList();
            var tourresults = new List<TourResult>();
            foreach (var tourevent in tourevents)
            {
                var toureventresults = (from toureventresult in _context.TourResult
                                       where toureventresult.TourEventId == tourevent.TourEventId
                                       select toureventresult).Include(ter => ter.User).ToList();
                tourresults.AddRange(toureventresults);
            }

            foreach (var tourresult in tourresults)
            {
                StandingsViewModel standing = standings[tourresult.User];
                PlacementRule placementrule = placementmap[tourresult.Place];
                standing.Score += placementrule.Points;
            }

            tvm.Standings = standings.Values.OrderByDescending(s => s.Score).ToList();

            return View(tvm);
        }

        // GET: Tour/TourPlayers/5
        public async Task<IActionResult> TourPlayers(int? id)
        {
            //id = HttpContext.Session.GetInt32("ChosenTourId");

            if (id == null)
            {
                return NotFound();
            }

            TourViewModel tvm = new TourViewModel();
            var players = (from player in _context.Users
                           where player.UserTours.Any(ut => ut.UserId.Equals(player.Id) && ut.TourId == id)
                           select player).ToList();
            var t = (from tour in _context.Tours
                      where tour.TourId == id
                     select tour).FirstOrDefault();
            tvm.Tour = t;
            tvm.TourMembers = players;

            return View(tvm);
        }

        // GET: Tour/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .SingleOrDefaultAsync(m => m.TourId == id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // Tour Create code

        // GET: Tour/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tour/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TourId,TourName,StartDate,EndDate,Colour")] Tour tour)
        {
            if (ModelState.IsValid)
            {
                string id = _userManager.GetUserId(User);
                ApplicationUser currentUser = await _userManager.FindByIdAsync(id);
                List<UserTour> tourMembers = new List<UserTour>();
                tourMembers.Add(new UserTour { Tour = tour, User = currentUser });
                tour.UserTours = tourMembers;
                _context.Add(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        // Tour Edit code

        // GET: Tour/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours.SingleOrDefaultAsync(m => m.TourId == id);
            if (tour == null)
            {
                return NotFound();
            }
            return View(tour);
        }

        // POST: Tour/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TourId,TourName,StartDate,EndDate,Colour")] Tour tour)
        {
            if (id != tour.TourId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tour.TourId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        // Tour Add and Remove members code

        public async Task<IActionResult> AddMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("ChosenTourId", (int)id);

            var av_members = (from player in _context.Users
                                where !(player.UserTours.Any(ut => ut.UserId.Equals(player.Id) && ut.TourId == id))
                                select player).ToList();

            AddMembersViewModel amvm = new AddMembersViewModel()
            {
                Tour = _context.Tours.Where(t => t.TourId == id).FirstOrDefault(),
                MemberList = new MultiSelectList(av_members, "Id", "UserName"),
                SelectedMembers = new List<string>()
            };

            return View(amvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMembers(AddMembersViewModel amvm)
        {
            Debug.WriteLine("Add Member: B4 Valid");
            if (ModelState.IsValid)
            {
                Debug.WriteLine("Add Member: Start");
                foreach (string member in amvm.SelectedMembers)
                {
                    Debug.WriteLine("Add Member: " + member);
                    var player = _context.Users.Where(u => u.Id.Equals(member)).FirstOrDefault();
                    var tour = _context.Tours.Include(ut => ut.UserTours).Where(t => t.TourId == HttpContext.Session.GetInt32("ChosenTourId")).FirstOrDefault();
                    tour.UserTours.Add(new UserTour { Tour = tour, User = player });
                    //tour.ToString();
                    _context.SaveChanges();
                }
                Debug.WriteLine("Add Member: End");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveMemberFromTour(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("RemoveMemberFromTour")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMemberFromTourConfirmed(string id)
        {
            Tour tour = _context.Tours.Where(t => t.TourId.Equals(
                HttpContext.Session.GetInt32("ChosenTourId"))).Include(t => t.UserTours).FirstOrDefault();

            UserTour usertour = tour.UserTours.Where(ut => ut.UserId.Equals(id)).First();
            tour.UserTours.Remove(usertour);

            await _context.SaveChangesAsync();

            return RedirectToAction("TourPlayers", new { id = HttpContext.Session.GetInt32("ChosenTourId") });
        }

        // Tour Delete Code

        // GET: Tour/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .SingleOrDefaultAsync(m => m.TourId == id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // POST: Tour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = await _context.Tours.SingleOrDefaultAsync(m => m.TourId == id);
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.TourId == id);
        }

        // Placementrules Controller Actions

        // GET: Tour/PlacementRules
        public async Task<IActionResult> PlacementRules(int? id)
        {
            TourViewModel tvm = new TourViewModel();
            var placementrules = (from placementrule in _context.PlacementRules
                                  where placementrule.TourId == id
                                  select placementrule).ToList();
            var t = (from tour in _context.Tours
                     where tour.TourId == id
                     select tour).FirstOrDefault();
            tvm.Tour = t;
            tvm.PlacementRules = placementrules;
            return View(tvm);
        }

        // GET: Tour/CreatePlacementRule
        public IActionResult CreatePlacementRule()
        {
            return View();
        }

        [HttpPost, ActionName("CreatePlacementRule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlacementRule(
            [Bind("PlacementRuleId,Place,Points")] PlacementRule placementrule)
        {
            Tour tour = _context.Tours.Where(t => t.TourId.Equals(
                HttpContext.Session.GetInt32("ChosenTourId"))).Include(t => t.PlacementRules).FirstOrDefault();

            if (EnsureUnique(placementrule.Place, tour.TourId))
            {
                // Need to put something in here that essentially says we can't 
                // cuz it's not unique.
                return NotFound();
            }

            tour.PlacementRules.Add(placementrule);
            _context.SaveChanges();

            return RedirectToAction("PlacementRules", new { id = tour.TourId });
        }

        public IActionResult DeletePlacementRule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placementrule = _context.PlacementRules.Where(pr => pr.PlacementRuleId == id)
                .FirstOrDefault();
            if (placementrule == null)
            {
                return NotFound();
            }

            return View(placementrule);
        }

        [HttpPost, ActionName("DeletePlacementRule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlacementRule(int id)
        {
            var placementrule = await _context.PlacementRules.SingleOrDefaultAsync(pr => pr.PlacementRuleId == id);
            _context.PlacementRules.Remove(placementrule);
            await _context.SaveChangesAsync();
            return RedirectToAction("PlacementRules", new { id = HttpContext.Session.GetInt32("ChosenTourId") });
        }

        private bool EnsureUnique(int place, int tourid)
        {
            return _context.PlacementRules.Any(pr => pr.Place == place 
                && pr.TourId == tourid);
        }

        // TourEvents code

        public async Task<IActionResult> TourEvents(int? id)
        {
            TourViewModel tvm = new TourViewModel();

            List<TourEvent> tourevents = (from tourevent in _context.TourEvents
                                          where tourevent.TourId == id
                                          select tourevent).ToList();
            var t = (from tour in _context.Tours
                     where tour.TourId == id
                     select tour).FirstOrDefault();
            tvm.Tour = t;
            tvm.TourEvents = tourevents;

            return View(tvm);
        }

        public IActionResult CreateTourEvent()
        {
            return View();
        }

        [HttpPost, ActionName("CreateTourEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTourEvent(
            [Bind("TourEventName,Location,Date")] TourEvent tourevent)
        {
            Tour tour = _context.Tours.Where(t => t.TourId.Equals(
                HttpContext.Session.GetInt32("ChosenTourId"))).Include(t => t.Events).FirstOrDefault();
            tourevent.Tour = tour;
            tour.Events.Add(tourevent);

            _context.SaveChanges();

            return RedirectToAction("TourEvents", new { id = tour.TourId });
        }

        public IActionResult DeleteTourEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tourevent = _context.TourEvents.Where(pr => pr.TourEventId == id)
                .FirstOrDefault();
            if (tourevent == null)
            {
                return NotFound();
            }

            return View(tourevent);
        }

        [HttpPost, ActionName("DeleteTourEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTourEvent(int id)
        {
            var tourevent = await _context.TourEvents.SingleOrDefaultAsync(pr => pr.TourEventId == id);
            _context.TourEvents.Remove(tourevent);
            await _context.SaveChangesAsync();
            return RedirectToAction("TourEvents", new { id = HttpContext.Session.GetInt32("ChosenTourId") });
        }

        public async Task<IActionResult> EditTourEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tourevent = await _context.TourEvents.SingleOrDefaultAsync(m => m.TourEventId == id);
            if (tourevent == null)
            {
                return NotFound();
            }
            return View(tourevent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTourEvent(int id, 
            [Bind("TourEventId,TourEventName,Location,Date")] TourEvent tourevent)
        {
            tourevent.TourId = (int)HttpContext.Session.GetInt32("ChosenTourId");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tourevent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction("TourEvents", new { id = HttpContext.Session.GetInt32("ChosenTourId") });
            }
            return View(tourevent);
        }

        // Eventresults code

            /*
        public IActionResult CreateTourEvent()
        {
            return View();
        }

        [HttpPost, ActionName("CreateTourEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTourEvent(
            [Bind("TourEventName,Location,Date")] TourEvent tourevent)
        {
            Tour tour = _context.Tours.Where(t => t.TourId.Equals(
                HttpContext.Session.GetInt32("ChosenTourId"))).Include(t => t.Events).FirstOrDefault();
            tourevent.Tour = tour;
            tour.Events.Add(tourevent);

            _context.SaveChanges();

            return RedirectToAction("TourEvents", new { id = tour.TourId });
        } */

        public IActionResult CreateTourResult()
        {
            var players = (from player in _context.Users
                           where player.UserTours.Any(ut => ut.UserId.Equals(player.Id) && ut.TourId == HttpContext.Session.GetInt32("ChosenTourId"))
                           select player).ToList();

            // ViewData["ActivityId"] = new SelectList(_context.Activities, "ActivityId", "ActivityDescription");
            ViewData["UserId"] = new SelectList(players, "UserId", "DisplayName");
            ViewBag.Members = new SelectList(players, "UserId", "DisplayName");
            ViewBag.Memberss = players;

            List<SelectListItem> playerlist = new List<SelectListItem>();

            foreach (ApplicationUser player in players)
            {
                playerlist.Add(
                    new SelectListItem
                    {
                        Text = player.DisplayName,
                        Value = player.Id
                    });
            }

            ViewBag.List = playerlist;

            Debug.WriteLine("Members Size: " + players.Count);

            // ViewBag.CityList = ToSelectList(_dt,"CityID","CityName");

            NewTourResultViewModel ntrvm = new NewTourResultViewModel();

            return View(ntrvm);
        }

        [HttpPost, ActionName("CreateTourResult")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTourResult(
            [Bind("UserId,Place")] NewTourResultViewModel ntrvm)
        {
            TourEvent tourevent = _context.TourEvents.Where(t => t.TourEventId.Equals(
                HttpContext.Session.GetInt32("ChosenEventId"))).Include(t => t.TourResults).FirstOrDefault();
            ApplicationUser user = _context.Users.Where(u => u.Id.Equals(ntrvm.UserId)).FirstOrDefault();
            /*
            Debug.WriteLine("Result Test (ID): " + tourresult.TourResultId);
            Debug.WriteLine("Result Test (UID): " + tourresult.User);
            Debug.WriteLine("Result Test (PLACE): " + tourresult.Place);

            tourevent.TourResults.Add(tourresult);
            _context.SaveChanges(); */

            Debug.WriteLine("Result Test (UID): " + ntrvm.UserId);
            Debug.WriteLine("Result Test (PLACE): " + ntrvm.Place);

            TourResult tr = new TourResult();
            tr.User = user;
            tr.Place = ntrvm.Place;

            tourevent.TourResults.Add(tr);
            _context.SaveChanges();

            return RedirectToAction("TourResults", new { id = HttpContext.Session.GetInt32("ChosenEventId") });
        }

        /*
        [HttpPost, ActionName("CreateTourResult")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTourResult(
            [Bind("TourResultId") */

        public async Task<IActionResult> TourResults(int? id)
        {
            HttpContext.Session.SetInt32("ChosenEventId", (int)id);

            TourViewModel tvm = new TourViewModel();
            
            /*
            List<TourResult> trs = new List<TourResult>();
            

            var tourevents = (from tourevent in _context.TourEvents
                              where tourevent.TourId == id
                              select tourevent).ToList();
            var tourresults = new List<TourResult>();
            foreach (var tourevent in tourevents)
            {
                var toureventresults = (from toureventresult in _context.TourResult
                                        where toureventresult.TourEventId == tourevent.TourEventId
                                        select toureventresult).Include(ter => ter.User).ToList();
                tourresults.AddRange(toureventresults);
            } */
            var tourresults = new List<TourResult>();

            var t = await _context.Tours.SingleOrDefaultAsync(m => m.TourId == HttpContext.Session.GetInt32("ChosenTourId"));

            var toureventresults = (from toureventresult in _context.TourResult
                                    where toureventresult.TourEventId == id
                                    select toureventresult).Include(ter => ter.User).ToList();
            tourresults.AddRange(toureventresults);

            tvm.TourResults = tourresults;
            tvm.Tour = t;

            return View(tvm);

            /*

            var tes = _context.TourEvents.Where(te => te.TourId == id).ToList();

            var t = (from tour in _context.Tours
                     where tour.TourId == id
                     select tour).FirstOrDefault();

            // Look through events and append results to trs and put trs as tvm's view
            foreach (TourEvent te in tes)
            {
                foreach (TourResult tr in te.TourResults)
                {
                    trs.Add(tr);
                }
            }

            tvm.Tour = t;
            tvm.TourResults = trs;

            return View(trs);

    */
        }

        public async Task<IActionResult> DeleteTourResult(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tr = _context.TourResult.Where(t => t.TourResultId == id).FirstOrDefault();

            if (tr == null)
            {
                return NotFound();
            }

            return View(tr);
        }

        [HttpPost, ActionName("DeleteTourResult")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTourResult(int id)
        {
            var tr = _context.TourResult.Where(t => t.TourResultId == id).FirstOrDefault();
            _context.TourResult.Remove(tr);
            _context.SaveChanges();

            return RedirectToAction("TourResults", new { id = HttpContext.Session.GetInt32("ChosenEventId") });
        }
    }
}


