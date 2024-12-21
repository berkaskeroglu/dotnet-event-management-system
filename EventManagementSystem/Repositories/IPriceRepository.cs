using EventManagementSystem.Models;

namespace EventManagementSystem.Repositories
{
    public interface IPriceRepository
    {
        Task<List<Price>> GetAllPricesAsync();
        Task<Price> AddPrice(int eventId, int category);
        Task<List<Price>> GetPriceByEventAsync(int eventId);
        Task<Price> GetPriceByEventAndCategoryAsync(int eventId, int category);
        Task UpdatePriceAsync(int eventId, int category, decimal newPrice);
    }

}
