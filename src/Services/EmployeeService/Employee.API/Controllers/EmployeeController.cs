using Microsoft.AspNetCore.Mvc;
using Employee.Application.Interfaces;
using Employee.Application.DTOs;

namespace Employee.API.Controllers;

[ApiController]
[Route("api/employee")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeeController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeDto dto)
    {
        await _service.Create(dto);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(EmployeeDto dto)
    {
        await _service.Update(dto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.Delete(id);
        return Ok();
    }
}