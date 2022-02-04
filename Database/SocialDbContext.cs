using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Database
{
    public class SocialDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Picture> Pictures { get; set; }

        public DbSet<Tag> Tags { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer("Server=.;Database=SocialNetworkDB;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Friendship>()
                   .HasKey(f => new { f.FromUserId, f.ToUserId });

            builder.Entity<User>()
                   .HasMany(u => u.FromFriends)
                   .WithOne(f => f.FromUser)
                   .HasForeignKey(f => f.FromUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                   .HasMany(u => u.ToFriends)
                   .WithOne(t => t.ToUser)
                   .HasForeignKey(t => t.ToUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                   .HasMany(u => u.Albums)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId);

            builder.Entity<AlbumPicture>()
                   .HasKey(ap => new { ap.PictureId, ap.AlbumId });

            builder.Entity<Picture>()
                   .HasMany(p => p.Albums)
                   .WithOne(a => a.Picture)
                   .HasForeignKey(a => a.PictureId);

            builder.Entity<Album>()
                   .HasMany(a => a.Pictures)
                   .WithOne(p => p.Album)
                   .HasForeignKey(p => p.AlbumId);

            builder.Entity<AlbumTag>()
                   .HasKey(at => new { at.AlbumId, at.TagId });

            builder.Entity<Album>()
                   .HasMany(a => a.Tags)
                   .WithOne(t => t.Album)
                   .HasForeignKey(t => t.AlbumId);

            builder.Entity<Tag>()
                   .HasMany(t => t.Albums)
                   .WithOne(a => a.Tag)
                   .HasForeignKey(a => a.TagId);               
        }

        public override int SaveChanges(bool accpetAllChangesOnSuccess)
        {
            var serviceProvider = this.GetService<IServiceProvider>();
            var items = new Dictionary<object, object>();

            foreach (var entry in this.ChangeTracker.Entries().Where(e => (e.State == EntityState.Added) || (e.State ==EntityState.Modified)))
            {
                var entity = entry.Entity;
                var context = new ValidationContext(entity, serviceProvider, items);
                var results = new List<ValidationResult>();

                if (Validator.TryValidateObject(entity, context, results, true) == false)
                {
                    foreach (var result in results)
                    {
                        if (result != ValidationResult.Success)
                        {
                            throw new ValidationException(result.ErrorMessage);
                        }
                    }
                }
            }
            return base.SaveChanges(accpetAllChangesOnSuccess);
        }

    }
}
