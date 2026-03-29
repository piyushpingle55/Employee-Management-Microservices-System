using Employee.Domain.Entities;
using EmployeeEntity = Employee.Domain.Entities.Employee;
using Employee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Employee.Infrastructure.Repositories;

public class EmployeeRepository
{
    private readonly EmployeeDbContext _context;

    public EmployeeRepository(EmployeeDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeEntity>> GetAll()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<EmployeeEntity?> GetById(Guid id)
    {
        return await _context.Employees.FindAsync(id);
    }

    public async Task Add(EmployeeEntity employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
    }

    public async Task Update(EmployeeEntity employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(EmployeeEntity employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}