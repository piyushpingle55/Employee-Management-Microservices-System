namespace  Employee.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public decimal Salary { get; set; }

    public Guid DepartmentId { get; set; }
}
