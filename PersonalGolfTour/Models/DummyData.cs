using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PersonalGolfTour.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models
{
    public class DummyData
    {
        public static void Initialize(ApplicationDbContext db)
        {
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            ApplicationUser[] users = SeedUsers(db);

            Tour[] tours = getTours();

            if (!db.Tours.Any())
            {
                foreach (Tour t in tours)
                {
                    db.Tours.Add(t);
                }
                db.SaveChanges();
                getUserTour(db);
                db.SaveChanges();
            }
            TourEvent[] tourEvents = getTourEvents(db);
            if (!db.TourEvents.Any())
            {
                foreach (TourEvent te in tourEvents)
                {
                    db.TourEvents.Add(te);
                }
                db.SaveChanges();
            }

            //getUserTour(db);
            db.SaveChanges();
        }

        private static ApplicationUser[] SeedUsers(ApplicationDbContext db)
        {
            ApplicationUser[] users =
            {
                new ApplicationUser
                {
                    UserName = "a@a.a",
                    FirstName = "Site",
                    LastName = "Admin",
                    Email = "a@a.a",
                    NormalizedUserName = "A@A.A",
                    NormalizedEmail = "A@A.A",
                    DisplayName = "Administrator1",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "m@m.m",
                    FirstName = "Site",
                    LastName = "Member",
                    Email = "m@m.m",
                    NormalizedUserName = "M@M.M",
                    NormalizedEmail = "M@M.M",
                    DisplayName = "Member1",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };
            /*
            }
            var admin = new ApplicationUser
            {
                UserName = "a@a.a",
                FirstName = "Site",
                LastName = "Admin",
                Email = "a@a.a",
                NormalizedUserName = "A@A.A",
                NormalizedEmail = "A@A.A",
                DisplayName = "Administrator1",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var member = new ApplicationUser
            {
                UserName = "m@m.m",
                FirstName = "Site",
                LastName = "Member",
                Email = "m@m.m",
                NormalizedUserName = "M@M.M",
                NormalizedEmail = "M@M.M",
                DisplayName = "Member1",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            }; */

            /*
            if (db.Users.Any(u => u.Email == "a@a.a"))
            {
                db.Users.Remove(db.Users.FirstOrDefault(a => a.Email == "a@a.a"));
                Debug.WriteLine("Removed Admin");
            }
            if (db.Users.Any(u => u.Email == "m@m.m"))
            {
                db.Users.Remove(db.Users.FirstOrDefault(a => a.Email == "m@m.m"));
                Debug.WriteLine("Removed Member");
            } */

            
            var roleStore = new RoleStore<IdentityRole>(db);
            /*
            if (!db.Roles.Any(r => r.Name.Equals("Admin")))
            {
                var x = roleStore.CreateAsync(new IdentityRole { Name = "Admin"}).Result;
            }

            if (!db.Roles.Any(r => r.Name.Equals("Member")))
            {
                var x = roleStore.CreateAsync(new IdentityRole { Name = "Member"}).Result;
            } */

            var z = roleStore.CreateAsync(new IdentityRole { Name = "Admin" }).Result;
            var y = roleStore.CreateAsync(new IdentityRole { Name = "Member" }).Result;

            //db.SaveChanges();
            
            if (!db.Users.Any(u => u.Email == "a@a.a"))
            {
                Debug.WriteLine("Attempting to create admin");
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(users[0], "P@$$w0rd");
                users[0].PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(db);
                var x = userStore.CreateAsync(users[0]).Result;

                //userStore.AddToRoleAsync(users[0], "Admin").Wait();
                //userStore.AddToRoleAsync(users[0], "Member").Wait();
            }

            if (!db.Users.Any(u => u.Email == "m@m.m"))
            {
                Debug.WriteLine("Attempting to create member");
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(users[1], "P@$$w0rd");
                users[1].PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(db);
                var x = userStore.CreateAsync(users[1]).Result;

                //userStore.AddToRoleAsync(users[1], "Member").Wait();
            }
            var res = db.SaveChangesAsync().Result;
            Debug.WriteLine("Finished seeding users");
            return users;
        }

        private static Tour[] getTours()
        {
            PlacementRule[] seedrules1 =
            {
                new PlacementRule
                {
                    Place = 1,
                    Points = 3
                },
                new PlacementRule
                {
                    Place = 2,
                    Points = 1
                }
            };
            PlacementRule[] seedrules2 =
            {
                new PlacementRule
                {
                    Place = 1,
                    Points = 5
                },
                new PlacementRule
                {
                    Place = 2,
                    Points = 3
                }
            };
            PlacementRule[] seedrules3 =
            {
                new PlacementRule
                {
                    Place = 1,
                    Points = 10
                },
                new PlacementRule
                {
                    Place = 2,
                    Points = 8
                }
            };
            List<PlacementRule> rules1 = new List<PlacementRule>(seedrules1);
            List<PlacementRule> rules2 = new List<PlacementRule>(seedrules2);
            List<PlacementRule> rules3 = new List<PlacementRule>(seedrules3);

            Tour[] tours = new Tour[]
            {
                new Tour()
                {
                    TourName = "Tour1",
                    StartDate = new DateTime(2018, 06, 01),
                    EndDate = new DateTime(2018, 09, 01),
                    Colour = "Blue",
                    PlacementRules = rules1
                },
                new Tour()
                {
                    TourName = "Tour2",
                    StartDate = new DateTime(2018, 05, 01),
                    EndDate = new DateTime(2018, 07, 01),
                    Colour = "Purple",
                    PlacementRules = rules2
                },
                new Tour()
                {
                    TourName = "Tour3",
                    StartDate = new DateTime(2018, 06, 01),
                    EndDate = new DateTime(2018, 07, 01),
                    Colour = "Red",
                    PlacementRules = rules3
                }
            };

            return tours;
        }

        private static TourEvent[] getTourEvents(ApplicationDbContext context)
        {
            TourResult[] seedresults1 = {
                new TourResult
                {
                    Place = 1,
                    User = context.Users.FirstOrDefault(m => m.Email == "a@a.a")
                },
                new TourResult
                {
                    Place = 2,
                    User = context.Users.FirstOrDefault(m => m.Email == "m@m.m")
                }
            };
            TourResult[] seedresults2 = {
                new TourResult
                {
                    Place = 1,
                    User = context.Users.FirstOrDefault(m => m.Email == "a@a.a")
                },
                new TourResult
                {
                    Place = 2,
                    User = context.Users.FirstOrDefault(m => m.Email == "m@m.m")
                }
            };
            TourResult[] seedresults3 = {
                new TourResult
                {
                    Place = 1,
                    User = context.Users.FirstOrDefault(m => m.Email == "a@a.a")
                },
                new TourResult
                {
                    Place = 2,
                    User = context.Users.FirstOrDefault(m => m.Email == "m@m.m")
                }
            };
            TourResult[] seedresults4 = {
                new TourResult
                {
                    Place = 1,
                    User = context.Users.FirstOrDefault(m => m.Email == "a@a.a")
                },
                new TourResult
                {
                    Place = 2,
                    User = context.Users.FirstOrDefault(m => m.Email == "m@m.m")
                }
            };
            TourResult[] seedresults5 = {
                new TourResult
                {
                    Place = 1,
                    User = context.Users.FirstOrDefault(m => m.Email == "a@a.a")
                },
                new TourResult
                {
                    Place = 2,
                    User = context.Users.FirstOrDefault(m => m.Email == "m@m.m")
                }
            };
            TourResult[] seedresults6 = {
                new TourResult
                {
                    Place = 1,
                    User = context.Users.FirstOrDefault(m => m.Email == "a@a.a")
                },
                new TourResult
                {
                    Place = 2,
                    User = context.Users.FirstOrDefault(m => m.Email == "m@m.m")
                }
            };
            TourResult[] seedresults7 = {
                new TourResult
                {
                    Place = 1,
                    User = context.Users.FirstOrDefault(m => m.Email == "a@a.a")
                },
                new TourResult
                {
                    Place = 2,
                    User = context.Users.FirstOrDefault(m => m.Email == "m@m.m")
                }
            };
            TourResult[] seedresults8 = {
                new TourResult
                {
                    Place = 1,
                    User = context.Users.FirstOrDefault(m => m.Email == "a@a.a")
                },
                new TourResult
                {
                    Place = 2,
                    User = context.Users.FirstOrDefault(m => m.Email == "m@m.m")
                }
            };
            TourResult[] seedresults9 = {
                new TourResult
                {
                    Place = 1,
                    User = context.Users.FirstOrDefault(m => m.Email == "a@a.a")
                },
                new TourResult
                {
                    Place = 2,
                    User = context.Users.FirstOrDefault(m => m.Email == "m@m.m")
                }
            };

            List<TourResult> results1 = new List<TourResult>(seedresults1);
            List<TourResult> results2 = new List<TourResult>(seedresults2);
            List<TourResult> results3 = new List<TourResult>(seedresults3);
            List<TourResult> results4 = new List<TourResult>(seedresults4);
            List<TourResult> results5 = new List<TourResult>(seedresults5);
            List<TourResult> results6 = new List<TourResult>(seedresults6);
            List<TourResult> results7 = new List<TourResult>(seedresults7);
            List<TourResult> results8 = new List<TourResult>(seedresults8);
            List<TourResult> results9 = new List<TourResult>(seedresults9);

            TourEvent[] tourEvents = new TourEvent[]
            {
                new TourEvent()
                {
                    TourEventName = "TourEvent1_1",
                    Location = "Surrey Golf Course",
                    Date = new DateTime(2018, 06, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour1").TourId,
                    TourResults = results1
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent1_2",
                    Location = "Delta Golf Course",
                    Date = new DateTime(2018, 07, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour1").TourId,
                    TourResults = results2
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent1_3",
                    Location = "Guildford Golf Course",
                    Date = new DateTime(2018, 09, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour1").TourId,
                    TourResults = results3
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent2_1",
                    Location = "Surrey Golf Course",
                    Date = new DateTime(2018, 05, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour2").TourId,
                    TourResults = results4
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent2_2",
                    Location = "Delta Golf Course",
                    Date = new DateTime(2018, 06, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour2").TourId,
                    TourResults = results5
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent2_3",
                    Location = "Guildford Golf Course",
                    Date = new DateTime(2018, 07, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour2").TourId,
                    TourResults = results6
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent3_1",
                    Location = "Surrey Golf Course",
                    Date = new DateTime(2018, 06, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour3").TourId,
                    TourResults = results7
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent3_2",
                    Location = "Delta Golf Course",
                    Date = new DateTime(2018, 06, 14),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour3").TourId,
                    TourResults = results8
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent3_3",
                    Location = "Guildford Golf Course",
                    Date = new DateTime(2018, 07, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour3").TourId,
                    TourResults = results9
                }
            };

            return tourEvents;
        }

        private static void getUserTour(ApplicationDbContext context)
        {
            Tour tour1 = context.Tours.FirstOrDefault(t => t.TourName == "Tour1");
            Tour tour2 = context.Tours.FirstOrDefault(t => t.TourName == "Tour2");
            Tour tour3 = context.Tours.FirstOrDefault(t => t.TourName == "Tour3");
            ApplicationUser adminUser = context.Users.FirstOrDefault(u => u.NormalizedEmail == "A@A.A");
            ApplicationUser memberUser = context.Users.FirstOrDefault(u => u.NormalizedEmail == "M@M.M");
            List<UserTour> tour1Tours = new List<UserTour>();
            List<UserTour> tour2Tours = new List<UserTour>();
            List<UserTour> tour3Tours = new List<UserTour>();
            tour1Tours.Add(new UserTour { Tour = tour1, User = adminUser });
            tour1Tours.Add(new UserTour { Tour = tour1, User = memberUser });
            tour2Tours.Add(new UserTour { Tour = tour2, User = adminUser });
            tour2Tours.Add(new UserTour { Tour = tour2, User = memberUser });
            tour3Tours.Add(new UserTour { Tour = tour3, User = adminUser });
            tour3Tours.Add(new UserTour { Tour = tour3, User = memberUser });
            tour1.UserTours = tour1Tours;
            tour2.UserTours = tour2Tours;
            tour3.UserTours = tour3Tours;
            //tour1.UserTours.Add(new UserTour { Tour = tour1, User = adminUser });
            //tour2.UserTours.Add(new UserTour { Tour = tour2, User = adminUser });
            //tour2.UserTours.Add(new UserTour { Tour = tour2, User = memberUser });
            //tour3.UserTours.Add(new UserTour { Tour = tour3, User = memberUser });
        }
    }
}
