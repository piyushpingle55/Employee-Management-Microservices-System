using Employee.Application.DTOs;

namespace Employee.Application.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAll();
    Task<EmployeeDto> GetById(Guid id);
    Task Create(EmployeeDto dto);
    Task Update(EmployeeDto dto);
    Task Delete(Guid id);
}