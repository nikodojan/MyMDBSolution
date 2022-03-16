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
            //var result = await _context.TitleBasicInfos.Where(t => t.PrimaryTitle == title).ToListAsync();
            return result;
        }

        public async Task<List<WriterOrDirector>> GetWritersAndDirectors(string tconst)
        {
            var result = await _context.GetWritersAndDirectorsByTconst(tconst).ToListAsync();
            return result;
        }
    }
}
