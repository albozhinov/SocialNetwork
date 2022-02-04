using Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork
{
    public class Program
    {
        private static Random random = new Random();


        static void Main()
        {
            using (var db = new SocialDbContext())
            {
                db.Database.Migrate();

                //SeedUsers(db);

                //AllUsersWithTheirFriendsCount(db);
                //AllActiveUsersWithMoreFiveFriends(db);
                //SeedAlbumsAndPicture(db);
                //PrintAllAlbumsWithOwnersNameAndPictureCount(db);
                //PrintPictureThatAreIncludeThanTwoAlbums(db);
                //PrintAllAlbums(db);

            }
        }

        private static void PrintAllAlbums(SocialDbContext db)
        {
            var albums = db.Albums
                .OrderBy(a => a.Name)
                .Select(a => new
            {
                UserName = a.User.Username,
                a.Public,
                Name = a.Name,
                Pictures = a.Pictures.Select(p => new 
                {
                    Title = p.Picture.Title,
                    Path = p.Picture.Path
                }),
            })
                .ToList();

            foreach (var album in albums)
            {
                Console.WriteLine($"Owner's name: {album.UserName};");

                if (!album.Public)
                {
                    Console.WriteLine($"Album's name: {album.Name} - Private content!");
                }
                else
                {
                    Console.WriteLine($"Album's name: {album.Name};");
                    foreach (var picture in album.Pictures)
                    {
                        Console.WriteLine($"Pictures: {picture}; Picture path: {picture.Path}");
                    }
                }                

                Console.WriteLine(new string('=', 50));
            }
        }

        private static void PrintPictureThatAreIncludeThanTwoAlbums(SocialDbContext db)
        {
            var pictures = db.Pictures
                             .Where(p => p.Albums.Count > 2)
                             .OrderByDescending(p => p.Albums.Count)
                             .ThenBy(p => p.Title)
                             .Select(p => new
                             {
                                 Title = p.Title,
                                 AlbumName = p.Albums.Select(a => new { Name = a.Album.Name, Owner = a.Album.User.Username}),
                             })
                             .ToList();

            foreach (var picture in pictures)
            {
                Console.WriteLine($"Title: {picture.Title}; Number of albums: {picture.AlbumName.Count()}");

                foreach (var album in picture.AlbumName)
                {
                    Console.WriteLine($"Albums's name: {album.Name}; Owner's name: {album.Owner}");
                }

                Console.WriteLine(new string('=', 30));
            }
        }

        private static void PrintAllAlbumsWithOwnersNameAndPictureCount(SocialDbContext db)
        {
            var allAlbums = db.Albums
                              .OrderByDescending(a => a.Pictures.Count)
                              .ThenBy(a => a.User.Username)
                              .Select(a => new
                              {
                                  OwnerName = a.User.Username,
                                  PictureCount = a.Pictures.Count,
                                  Title = a.Name
                              })
                              .ToList();

            foreach (var album in allAlbums)
            {
                Console.WriteLine($"Title: {album.Title}; Owner: {album.OwnerName}; Number of pictures: {album.PictureCount}");
                Console.WriteLine(new string('=', 25));
            }
        }

        private static void AllActiveUsersWithMoreFiveFriends(SocialDbContext db)
        {
            var allUsers = db.Users
                             .Where(u => !u.IsDeleted && (u.FromFriends.Count + u.ToFriends.Count) > 15)
                             .Select(u => new
                             {
                                 u.Username,
                                 FriendsCount = u.FromFriends.Count + u.ToFriends.Count,
                                 u.RegisteredOn,
                                 Period =  u.RegisteredOn - DateTime.Now
                             })
                             .OrderBy(u => u.RegisteredOn)
                             .ThenByDescending(u => u.FriendsCount)
                             .ToList();
            foreach (var user in allUsers)
            {
                Console.WriteLine($"Name: {user.Username}; FriendsCount: {user.FriendsCount}; Period: {user.Period.ToString()}");

                Console.WriteLine("=================================");
            }

        }

        private static void AllUsersWithTheirFriendsCount(SocialDbContext db)
        {
            var allUsers = db.Users
                             .Select(u => new
                             {
                                 u.Username,
                                 FriendsCount = u.FromFriends.Count + u.ToFriends.Count,
                                 u.IsDeleted
                             })
                             .OrderByDescending(u => u.FriendsCount)
                             .ThenBy(u => u.Username)
                             .ToList();

            foreach (var user in allUsers)
            {
                var userStatus = "Active";

                if (user.IsDeleted)
                {
                    userStatus = "Inactive";
                }
                Console.WriteLine($"Username: {user.Username}; FriendsCount: {user.FriendsCount}; Status: {userStatus}");
                Console.WriteLine("===========================");
            }
        }

        private static void SeedUsers(SocialDbContext db)
        {
            const int totalUsers = 50;
            var biggestUserId = db.Users
                                  .OrderByDescending(u => u.Id)
                                  .Select(u => u.Id)
                                  .FirstOrDefault() + 1;

            var allUsers = new List<User>();

            for (int i = biggestUserId; i < biggestUserId + totalUsers; i++)
            {
                var user = new User()
                {
                    Username = $"Username {i}",
                    Password = $"Passw0rd#$",
                    Email = $"email@email{i}.com",
                    RegisteredOn = DateTime.Now.AddDays(100 + i * 10),
                    LastTimeLoggedIn = DateTime.Now.AddDays(i),
                    Age = i
                };

                allUsers.Add(user);
                db.Users.Add(user);

            }
            db.SaveChanges();

            var userIds = allUsers.Select(u => u.Id).ToList();

            for (int i = 0; i < userIds.Count; i++)
            {
                var currUserId = userIds[i];

                var totalFriends = random.Next(5, 11);

                for (int j = 0; j < totalFriends; j++)
                {
                    var friendUserId = userIds[random.Next(0, userIds.Count)];

                    var validFriendship = true;

                    // Cannot be friend to myself
                    if (friendUserId == currUserId)
                    {
                        validFriendship = false;
                    }
                    var friendShipExist = db
                              .Friendships
                              .Any(f => (f.FromUserId == currUserId && f.ToUserId == friendUserId) || (f.FromUserId == friendUserId && f.ToUserId == currUserId));

                    if (friendShipExist)
                    {
                        validFriendship = false;
                    }

                    if (!validFriendship)
                    {
                        j--;
                        continue;
                    }
                    db.Friendships.Add(new Friendship
                    {
                        FromUserId = currUserId,
                        ToUserId = friendUserId
                    });

                    db.SaveChanges();
                }
            }
        }

        private static void SeedAlbumsAndPicture(SocialDbContext db)
        {
            const int totalAlbums = 50;
            const int totalPictures = 200;

            var biggestAlbumId = db
                    .Albums
                    .OrderByDescending(a => a.Id)
                    .Select(a => a.Id)
                    .FirstOrDefault() + 1;

            var userId = db.Users
                           .Select(u => u.Id)
                           .ToList();

            var albums = new List<Album>();

            for (int i = biggestAlbumId; i < totalAlbums; i++)
            {
                var album = new Album
                {
                    Name = $"Albums {i}",
                    BackgroundColor = $"Color {i}",
                    Public = random.Next(0, 2) == 0 ? true : false,
                    UserId = userId[random.Next(0, userId.Count)]
                };

            albums.Add(album);
            db.Albums.Add(album);
            }

            db.SaveChanges();

            var biggestPictureId = db
                        .Pictures
                        .OrderByDescending(p => p.Id)
                        .Select(p => p.Id)
                        .FirstOrDefault() + 1;

            var pictures = new List<Picture>();

            for (int i = biggestPictureId; i < totalPictures; i++)
            {
                var picture = new Picture
                {
                    Title = $"Picture {i}",
                    Caption = $"Caption {i}",
                    Path = $"Path {i}"
                };

                pictures.Add(picture);
                db.Pictures.Add(picture);
            }

            db.SaveChanges();

            var albumIds = albums.Select(a => a.Id).ToList();
            var pictereIds = pictures.Select(p => p.Id).ToList();

            for (int i = 0; i < pictures.Count; i++)
            {
                var numberOfAlbums = random.Next(1, 5);
                var picture = pictures[i];                

                for (int j = 0; j < numberOfAlbums; j++)
                {
                    var albumId = albumIds[random.Next(0, albumIds.Count)];

                    var pictireExistInAlbum = db
                            .Pictures
                            .Any(p => p.Id == picture.Id && p.Albums.Any(a => a.AlbumId == albumId));
                    if (pictireExistInAlbum)
                    {
                        j--;
                        continue;
                    }
                    picture.Albums.Add(new AlbumPicture
                    {
                        AlbumId = albumId,
                    });

                    db.SaveChanges();
                }
            }
        }
    }
}
