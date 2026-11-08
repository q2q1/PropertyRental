using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;

namespace PropertyRental.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class ApartmentsApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ApartmentsApiController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>Public read: list apartments (for search integration).</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] string? city, [FromQuery] decimal? maxRent)
    {
        var q = _db.Apartments
            .Include(a => a.Building)
            .Where(a => a.Status == ApartmentStatus.Available || a.Status == ApartmentStatus.Reserved)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(city))
            q = q.Where(a => a.Building!.City != null && a.Building.City.Contains(city));
        if (maxRent.HasValue)
            q = q.Where(a => a.MonthlyRent <= maxRent.Value);

        var data = await q
            .OrderBy(a => a.MonthlyRent)
            .Select(a => new
            {
                a.Id,
                a.UnitNumber,
                a.Bedrooms,
                a.Bathrooms,
                a.MonthlyRent,
                Status = a.Status.ToString(),
                a.Description,
                BuildingName = a.Building!.Name,
                City = a.Building.City
            })
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var a = await _db.Apartments
            .Include(x => x.Building)
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.UnitNumber,
                x.Bedrooms,
                x.Bathrooms,
                x.MonthlyRent,
                Status = x.Status.ToString(),
                x.Description,
                x.ImageUrl,
                BuildingName = x.Building!.Name,
                City = x.Building.City,
                Address = x.Building.StreetAddress
            })
            .FirstOrDefaultAsync();
        if (a == null)
            return NotFound();
        return Ok(a);
    }

    [HttpPost]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Post([FromBody] Apartment body)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var ok = await _db.Buildings.AnyAsync(b =>
            b.Id == body.BuildingId && b.PropertyManagerId == uid);
        if (!ok)
            return Forbid();
        _db.Apartments.Add(body);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = body.Id }, body);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Put(int id, [FromBody] Apartment body)
    {
        if (id != body.Id)
            return BadRequest();
        var existing = await _db.Apartments
            .Include(a => a.Building)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (existing == null)
            return NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (existing.Building!.PropertyManagerId != userId)
            return Forbid();
        existing.UnitNumber = body.UnitNumber;
        existing.Bedrooms = body.Bedrooms;
        existing.Bathrooms = body.Bathrooms;
        existing.MonthlyRent = body.MonthlyRent;
        existing.Status = body.Status;
        existing.Description = body.Description;
        existing.ImageUrl = body.ImageUrl;
        existing.BuildingId = body.BuildingId;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Apartments
            .Include(a => a.Building)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (existing == null)
            return NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (existing.Building!.PropertyManagerId != userId)
            return Forbid();
        _db.Apartments.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
