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
            ApplicationUser[] users = SeedUsers(db);

            Tour[] tours = getTours();

            if (!db.Tours.Any())
            {
                foreach (Tour t in tours)
                {
                    db.Tours.Add(t);
                }
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
            if (!db.Roles.Any(r => r.Name.Equals("Admin")))
            {
                var x = roleStore.CreateAsync(new IdentityRole { Name = "Admin"}).Result;
            }

            if (!db.Roles.Any(r => r.Name.Equals("Member")))
            {
                var x = roleStore.CreateAsync(new IdentityRole { Name = "Member", NormalizedName = "member" }).Result;
            }

            
            if (!db.Users.Any(u => u.Email == "a@a.a"))
            {
                Debug.WriteLine("Attempting to create admin");
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(users[0], "P@$$w0rd");
                users[0].PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(db);
                var x = userStore.CreateAsync(users[0]).Result;

                userStore.AddToRoleAsync(users[0], "Admin").Wait();
                userStore.AddToRoleAsync(users[0], "Member").Wait();
            }

            if (!db.Users.Any(u => u.Email == "m@m.m"))
            {
                Debug.WriteLine("Attempting to create member");
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(users[1], "P@$$w0rd");
                users[1].PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(db);
                var x = userStore.CreateAsync(users[1]).Result;

                userStore.AddToRoleAsync(users[1], "Member").Wait();
            }
            var res = db.SaveChangesAsync().Result;
            Debug.WriteLine("Finished seeding users");
            return users;
        }

        private static Tour[] getTours()
        {
            Tour[] tours = new Tour[]
            {
                new Tour()
                {
                    TourName = "Tour1",
                    StartDate = new DateTime(2018, 06, 01),
                    EndDate = new DateTime(2018, 09, 01),
                    Colour = "Blue"
                },
                new Tour()
                {
                    TourName = "Tour2",
                    StartDate = new DateTime(2018, 05, 01),
                    EndDate = new DateTime(2018, 07, 01),
                    Colour = "Purple"
                },
                new Tour()
                {
                    TourName = "Tour3",
                    StartDate = new DateTime(2018, 06, 01),
                    EndDate = new DateTime(2018, 07, 01),
                    Colour = "Red"
                }
            };

            return tours;
        }

        private static TourEvent[] getTourEvents(ApplicationDbContext context)
        {
            TourEvent[] tourEvents = new TourEvent[]
            {
                new TourEvent()
                {
                    TourEventName = "TourEvent1_1",
                    Location = "Surrey Golf Course",
                    Date = new DateTime(2018, 06, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour1").TourId
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent1_2",
                    Location = "Delta Golf Course",
                    Date = new DateTime(2018, 07, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour1").TourId
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent1_3",
                    Location = "Guildford Golf Course",
                    Date = new DateTime(2018, 09, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour1").TourId
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent2_1",
                    Location = "Surrey Golf Course",
                    Date = new DateTime(2018, 05, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour2").TourId
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent2_2",
                    Location = "Delta Golf Course",
                    Date = new DateTime(2018, 06, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour2").TourId
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent2_3",
                    Location = "Guildford Golf Course",
                    Date = new DateTime(2018, 07, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour2").TourId
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent3_1",
                    Location = "Surrey Golf Course",
                    Date = new DateTime(2018, 06, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour3").TourId
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent3_2",
                    Location = "Delta Golf Course",
                    Date = new DateTime(2018, 06, 14),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour3").TourId
                },
                new TourEvent()
                {
                    TourEventName = "TourEvent3_3",
                    Location = "Guildford Golf Course",
                    Date = new DateTime(2018, 07, 01),
                    TourId = context.Tours.FirstOrDefault(a => a.TourName == "Tour3").TourId
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
            tour1.UserTours.Add(new UserTour { Tour = tour1, User = adminUser });
            tour2.UserTours.Add(new UserTour { Tour = tour2, User = adminUser });
            tour2.UserTours.Add(new UserTour { Tour = tour2, User = memberUser });
            tour3.UserTours.Add(new UserTour { Tour = tour3, User = memberUser });
        }
    }
}
