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
    }
}


