using EventManagementSystem.Models;
using EventManagementSystem.Repositories;

namespace EventManagementSystem.Services
{
    public class PriceService : IPriceService
    {
        private readonly IPriceRepository _priceRepository;

        public PriceService(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public async Task<List<Price>> GetAllPricesAsync()
        {
            return await _priceRepository.GetAllPricesAsync();
        }

        public async Task<Price> AddPrice(int eventId, int category)
        {
            return await _priceRepository.AddPrice(eventId, category);
        }

        public async Task<List<Price>> GetPriceByEventAsync(int eventId)
        {
            return await _priceRepository.GetPriceByEventAsync(eventId);
        }

        public async Task<Price> GetPriceByEventAndCategoryAsync(int eventId, int category)
        {
            return await _priceRepository.GetPriceByEventAndCategoryAsync(eventId, category);
        }

        public async Task UpdatePriceAsync(int eventId, int category, decimal newPrice)
        {
            await _priceRepository.UpdatePriceAsync(eventId, category, newPrice);
        }
    }

}
