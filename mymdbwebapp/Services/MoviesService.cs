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
    }
}
