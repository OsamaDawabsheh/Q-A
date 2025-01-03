using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectAPI_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

            builder.Entity<IdentityRole<int>>().HasData(
          new IdentityRole<int>
          {
              Id = 1, 
              Name = "Admin",
              NormalizedName = "ADMIN"
          },
          new IdentityRole<int>
          {
              Id = 2, 
              Name = "User",
              NormalizedName = "USER"
          }
      );


            builder.Entity<QuestionTag>().HasKey(qt => new { qt.QuestionId, qt.TagId });
            builder.Entity<QuestionUsefulness>().HasKey(qu => new { qu.UserId, qu.QuestionId });
            builder.Entity<AnswerUsefulness>().HasKey(au => new { au.UserId, au.AnswerId });


                    builder.Entity<Comment>()
            .HasOne(c => c.parentComment)
            .WithMany(c => c.replies)
            .HasForeignKey(c => c.ParentCommentId)
            .IsRequired(false);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<QuestionTag> QuestionTags { get; set; }
        public DbSet<QuestionUsefulness> QuestionUsefulnesses { get; set; }
        public DbSet<AnswerUsefulness> AnswerUsefulnesses { get; set; }



    }
}
