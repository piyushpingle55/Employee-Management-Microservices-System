namespace Employee.Application.DTOs;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal Salary { get; set; }
}