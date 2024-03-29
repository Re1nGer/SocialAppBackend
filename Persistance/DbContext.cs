﻿
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserPost> UserPosts { get; set; }
        public virtual DbSet<UserChat> UserChats { get; set; }
        public virtual DbSet<Comment> UserComments { get; set; }
        public virtual DbSet<Like> UserLikes { get; set; }
        public virtual DbSet<PostBookmark> PostBookmarks { get; set; }
        public virtual DbSet<UserRequest> UserRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }

}
