using Microsoft.EntityFrameworkCore;
using mymdbwebapp.Data;
using mymdbwebapp.Models.ViewModels;

namespace mymdbwebapp.Services
{
    public class MoviesService
    {
        private readonly MyMdbContext _context;

        public MoviesService(MyMdbContext ctx)
        {
            _context = ctx;
        }

        public List<TitleBasicInfo> GetMovies(int amount)
        {
            return _context.TitleBasicInfos.Take(amount).ToList();
        }

        public async Task<TitleBasicInfo> GetMovieByTconst(string tconst)
        {
            return await _context.TitleBasicInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Tconst == tconst);
        }

        public async Task<List<TitleBasicInfo>> SearchByTitle(string title)
        {
            var result = await _context.FindTitlesContainingWord(title).OrderBy(t=>t.PrimaryTitle).ToListAsync();
            return result;
        }

        public async Task<List<WriterOrDirector>> GetWritersAndDirectors(string tconst)
        {
            var result = await _context.GetWritersAndDirectorsByTconst(tconst).ToListAsync();
            return result;
        }


        
        public async Task InsertIntoTitles(int id)
        {
            string tconst = $"testConst{id}";
            string title = "PTestTitle";
            _context.Database.ExecuteSqlInterpolated($"EXEC InsertIntoTitleBasics @Tconst = {tconst}, @TitleType = 1, @PrimaryTitle = {title}, @OriginalTitle = 'OTestTitle', @IsAdult = 1, @StartYear = 2000, @EndYear = NULL, @Runtmie = 24");
        }
    }
}
