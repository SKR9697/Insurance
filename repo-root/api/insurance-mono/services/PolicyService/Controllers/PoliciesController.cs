using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PolicyService.Models;
using PolicyService.Repositories;
using System.Security.Claims;

namespace PolicyService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/policies")]
    public class PoliciesController : ControllerBase
    {
        private readonly IPolicyRepository _repo;
        public PoliciesController(IPolicyRepository repo) { _repo = repo; }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repo.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _repo.GetAsync(id);
            return p is null ? NotFound() : Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePolicyDto dto)
        {
            var userId = GetUserId(); // get user details from token
            var id = await _repo.CreateAsync(new Policy(
              0, dto.PolicyNumber, dto.Title, dto.Status, dto.PremiumAmount,
              dto.StartDate, dto.EndDate, dto.PolicyConfigId, userId, dto.CreatedAt
            ));
            return CreatedAtAction(nameof(Get), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePolicyDto dto)
        {
            var existing = await _repo.GetAsync(id);
            if (existing is null) return NotFound();
            // Optional: enforce owner-only updates
            var ok = await _repo.UpdateAsync(new Policy(
              id, existing.PolicyNumber, dto.Title, dto.Status,
              dto.PremiumAmount, dto.StartDate, dto.EndDate, dto.PolicyConfigId, existing.OwnerUserId,dto.CreatedAt
            ));
            return ok ? NoContent() : Problem("Update failed");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
