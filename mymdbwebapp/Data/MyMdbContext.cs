using Microsoft.EntityFrameworkCore;
using mymdbwebapp.Models.ViewModels;

namespace mymdbwebapp.Data
{
    public class MyMdbContext : DbContext
    {
        public MyMdbContext(DbContextOptions<MyMdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TitleBasicInfo> TitleBasicInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TitleBasicInfo>()
                .ToView("v_titleBasicInfo")
                .HasKey("Tconst");
            modelBuilder.HasDbFunction(typeof(MyMdbContext).GetMethod(nameof(FindTitlesContainingWord),
                new[] { typeof(string) })).HasName("findTitleWithWildcard");
        }

        public IQueryable<TitleBasicInfo> FindTitlesContainingWord(string title)
            => FromExpression(() => FindTitlesContainingWord(title));
    }
}
