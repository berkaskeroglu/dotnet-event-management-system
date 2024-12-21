using EventManagementSystem.Services;
using EventManagementSystem.Dto;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PriceController : ControllerBase
{
    private readonly IPriceService _priceService;

    public PriceController(IPriceService priceService)
    {
        _priceService = priceService;
    }

    // GET: api/Price
    [HttpGet]
    public async Task<IActionResult> GetAllPrices()
    {
        var prices = await _priceService.GetAllPricesAsync();
        return Ok(prices);
    }

    // GET: api/Price/event/{eventId}
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetPricesByEvent(int eventId)
    {
        var prices = await _priceService.GetPriceByEventAsync(eventId);
        return Ok(prices);
    }

    // GET: api/Price/event/{eventId}/category/{category}
    [HttpGet("event/{eventId}/category/{category}")]
    public async Task<IActionResult> GetPriceByEventAndCategory(int eventId, int category)
    {
        try
        {
            var price = await _priceService.GetPriceByEventAndCategoryAsync(eventId, category);
            return Ok(price);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Price not found for EventId: {eventId} and Category: {category}");
        }
    }

    // POST: api/Price
    [HttpPost]
    public async Task<IActionResult> AddPrice([FromBody] AddPriceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newPrice = await _priceService.AddPrice(request.EventId, request.Category);
        return CreatedAtAction(nameof(GetPriceByEventAndCategory), new { eventId = newPrice.EventId, category = newPrice.Category }, newPrice);
    }

    // PUT: api/Price/event/{eventId}/category/{category}
    [HttpPut("event/{eventId}/category/{category}")]
    public async Task<IActionResult> UpdatePrice(int eventId, int category, [FromBody] UpdatePriceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _priceService.UpdatePriceAsync(eventId, category, request.NewPrice);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Price not found for EventId: {eventId} and Category: {category}");
        }
    }
}
