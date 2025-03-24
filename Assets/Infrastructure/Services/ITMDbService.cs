using System.Threading.Tasks;
using FastTV.Data.Models;

namespace FastTV.Infrastructure.Services
{
    public interface ITMDbService
    {
        Task<SearchResult> SearchMoviesAsync(string query, int page = 1);
        Task<MovieData> GetMovieDetailsAsync(int movieId);
        void SetApiKey(string apiKey);
        bool HasValidApiKey();
    }
} 