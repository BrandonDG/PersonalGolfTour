using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

            //var placementrules = _context.PlacementRules.ToList();
            //_context.Tours.Where(t => t.TourId == id).Include(t => t.PlacementRules);
            return View(tvm);
        }

        public async Task<IActionResult> TourResults()
        {
            // Query all tour events for a tour
            // Then loop and query all tour results for tour events

            //var placementrules = _context.TourResult.ToList();

            return View(_context.TourResult.Include(u => u.User).Where(tr => tr.TourEvent.TourId == 1).ToList());
        }

        // GET: Tour/TourStandings/5
        public async Task<IActionResult> TourStandings(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*
             * Previous code, just finds a Tour based on TourId
            var tour = await _context.Tours
                .SingleOrDefaultAsync(m => m.TourId == id);
            if (tour == null)
            {
                return NotFound();
            } */

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
    }
}
