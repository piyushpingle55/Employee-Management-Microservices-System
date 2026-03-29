using Employee.Infrastructure.Repositories;
using Employee.Application.DTOs;
using Employee.Application.Interfaces;
using EmployeeEntity = Employee.Domain.Entities.Employee;

namespace Employee.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly EmployeeRepository _repo;

    public EmployeeService(EmployeeRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<EmployeeDto>> GetAll()
    {
        var data = await _repo.GetAll();

        return data.Select(x => new EmployeeDto
        {
            Id = x.Id,
            Name = x.Name,
            Email = x.Email,
            Salary = x.Salary
        }).ToList();
    }

    public async Task<EmployeeDto> GetById(Guid id)
    {
        var x = await _repo.GetById(id);
        if (x == null)
        {
            throw new KeyNotFoundException("Employee not found");
        }

        return new EmployeeDto
        {
            Id = x.Id,
            Name = x.Name,
            Email = x.Email,
            Salary = x.Salary
        };
    }

    public async Task Create(EmployeeDto dto)
    {
        var emp = new EmployeeEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Salary = dto.Salary
        };

        await _repo.Add(emp);
    }

    public async Task Update(EmployeeDto dto)
    {
        var emp = new EmployeeEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Salary = dto.Salary
        };

        await _repo.Update(emp);
    }

    public async Task Delete(Guid id)
    {
        var emp = await _repo.GetById(id);
        if (emp != null)
        {
            await _repo.Delete(emp);
        }
    }
}