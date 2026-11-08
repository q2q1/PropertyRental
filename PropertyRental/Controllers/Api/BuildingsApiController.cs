using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;

namespace PropertyRental.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class BuildingsApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public BuildingsApiController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.Buildings
            .OrderBy(b => b.Name)
            .Select(b => new
            {
                b.Id,
                b.Name,
                b.StreetAddress,
                b.City,
                b.PostalCode
            })
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var b = await _db.Buildings
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.StreetAddress,
                x.City,
                x.PostalCode
            })
            .FirstOrDefaultAsync();
        if (b == null)
            return NotFound();
        return Ok(b);
    }

    [HttpPost]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Post([FromBody] Building body)
    {
        body.PropertyManagerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        _db.Buildings.Add(body);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = body.Id }, body);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Put(int id, [FromBody] Building body)
    {
        if (id != body.Id)
            return BadRequest();
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = await _db.Buildings.FirstOrDefaultAsync(b => b.Id == id && b.PropertyManagerId == uid);
        if (existing == null)
            return NotFound();
        existing.Name = body.Name;
        existing.StreetAddress = body.StreetAddress;
        existing.City = body.City;
        existing.PostalCode = body.PostalCode;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Delete(int id)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = await _db.Buildings.FirstOrDefaultAsync(b => b.Id == id && b.PropertyManagerId == uid);
        if (existing == null)
            return NotFound();
        _db.Buildings.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
