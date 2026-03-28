
using Microsoft.EntityFrameworkCore;
using EmployeeEntity = Employee.Domain.Entities.Employee;

public class EmployeeDbContext : DbContext
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
        : base(options)
    {
    }

    public DbSet<EmployeeEntity> Employees { get; set; }
}