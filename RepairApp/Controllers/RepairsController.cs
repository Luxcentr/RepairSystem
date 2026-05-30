using Microsoft.AspNetCore.Mvc;
using RepairApp.Models;
using RepairApp.Services;

namespace RepairApp.Controllers;

[ApiController]
[Route("api/repairs")]
public class RepairsController : ControllerBase
{
    private readonly IRepairService _service;

    public RepairsController(IRepairService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<RepairOrder>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RepairOrder>> GetById(Guid id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order is null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<RepairOrder>> Create(RepairOrder order)
    {
        var created = await _service.CreateAsync(order);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, RepairOrder order)
    {
        var updated = await _service.UpdateAsync(id, order);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] RepairStatus status)
    {
        var changed = await _service.ChangeStatusAsync(id, status);
        if (!changed) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}