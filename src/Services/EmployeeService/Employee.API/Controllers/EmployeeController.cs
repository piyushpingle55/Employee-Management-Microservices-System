using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Employee.Application.Interfaces;
using Employee.Application.DTOs;

namespace Employee.API.Controllers;

/// <summary>
/// Employee management API endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _service;

    /// <summary>
    /// Initializes a new instance of the EmployeeController
    /// </summary>
    /// <param name="service">The employee service</param>
    public EmployeeController(IEmployeeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all employees
    /// </summary>
    /// <returns>List of all employees</returns>
    /// <response code="200">Returns list of employees</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    /// <summary>
    /// Retrieves an employee by ID
    /// </summary>
    /// <param name="id">The employee ID</param>
    /// <returns>Employee details</returns>
    /// <response code="200">Returns employee</response>
    /// <response code="404">Employee not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var emp = await _service.GetById(id);
            return Ok(emp);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Employee not found");
        }
    }

    /// <summary>
    /// Creates a new employee
    /// </summary>
    /// <param name="dto">Employee data</param>
    /// <returns>Success response</returns>
    /// <response code="200">Employee created successfully</response>
    /// <response code="400">Invalid employee data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EmployeeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        await _service.Create(dto);
        return Ok(new { message = "Employee created successfully" });
    }

    /// <summary>
    /// Updates an existing employee
    /// </summary>
    /// <param name="id">The employee ID to update</param>
    /// <param name="dto">Updated employee data</param>
    /// <returns>Success response</returns>
    /// <response code="200">Employee updated successfully</response>
    /// <response code="400">Invalid employee data</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        dto.Id = id;
        await _service.Update(dto);
        return Ok(new { message = "Employee updated successfully" });
    }

    /// <summary>
    /// Deletes an employee
    /// </summary>
    /// <param name="id">The employee ID to delete</param>
    /// <returns>Success response</returns>
    /// <response code="200">Employee deleted successfully</response>
    /// <response code="404">Employee not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.Delete(id);
            return Ok(new { message = "Employee deleted successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Employee not found");
        }
    }
}