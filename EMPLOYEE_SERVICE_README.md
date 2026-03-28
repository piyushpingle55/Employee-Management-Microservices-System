# Employee Service Architecture & Flow

Your Employee Service follows a **layered architecture pattern**. Here's the complete breakdown:

---

## 🏛️ Architecture Layers

```
┌─────────────────────────────────────┐
│    Employee.API (Controllers)       │ ← HTTP Requests
├─────────────────────────────────────┤
│  Employee.Application (Services)    │ ← Business Logic
├─────────────────────────────────────┤
│ Employee.Infrastructure (Repos)     │ ← Data Access
├─────────────────────────────────────┤
│  Employee.Domain (Entities)         │ ← Data Models
└─────────────────────────────────────┘
```

---

## 🧩 Component Breakdown

### 1. **IEmployeeService (Interface)**

```csharp
public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAll();
    Task<EmployeeDto> GetById(Guid id);
    Task Create(EmployeeDto dto);
    Task Update(EmployeeDto dto);
    Task Delete(Guid id);
}
```

**Why?**
- **Abstraction** - Decouples controllers from implementation
- **Dependency Injection** - Enables loose coupling between layers
- **Testability** - Easy to mock for unit tests

---

### 2. **EmployeeRepository (Data Access Layer)**

```csharp
public class EmployeeRepository
{
    private readonly EmployeeDbContext _context;

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
```

**Why?**
- **Single Responsibility** - Handles all database operations
- **Reusability** - Can be used by multiple services
- **Maintainability** - All SQL logic in one place
- **Abstraction** - Service doesn't know HOW data is retrieved

**Methods Used:**

| Method | Purpose |
|--------|---------|
| `GetAll()` | Fetch all employees |
| `GetById()` | Fetch single employee |
| `Add()` | Insert new record |
| `Update()` | Modify existing record |
| `Delete()` | Remove record |

---

### 3. **EmployeeDto (Data Transfer Object)**

```csharp
namespace Employee.Application.DTOs;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal Salary { get; set; }
}
```

**Why?**
- **API Contract** - Defines what client receives/sends
- **Security** - Don't expose internal domain models
- **Flexibility** - Can differ from domain entity
- **Serialization** - Clean JSON over HTTP

---

### 4. **EmployeeEntity (Domain Entity)**

```csharp
namespace Employee.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal Salary { get; set; }
    public Guid DepartmentId { get; set; }
}
```

**Why?**
- **Business Logic** - Represents "Employee" concept in code
- **Consistency** - Single source of truth for employee data
- **Namespace Collision Fix** - Avoids conflict with `Employee.Application` namespace

---

## 📊 Data Flow Example: GetById()

```
EmployeeController
    ↓ GetById(id)
EmployeeService.GetById(id)
    ↓ calls
EmployeeRepository.GetById(id)
    ↓ queries
DbContext.FindAsync<Employee>(id)
    ↓ 
SQL Server Database
    ↓ returns
EmployeeEntity
    ↓ maps to
EmployeeDto
    ↓ 
Controller → JSON Response → Client
```

---

## 💡 Full Flow Example: Create()

```csharp
public async Task Create(EmployeeDto dto)
{
    // 1. Convert DTO to Domain Entity
    var emp = new EmployeeEntity
    {
        Id = Guid.NewGuid(),           // Generate unique ID
        Name = dto.Name,                // Map from request
        Email = dto.Email,              // Map from request
        Salary = dto.Salary             // Map from request
    };

    // 2. Persist to database via repository
    await _repo.Add(emp);
}
```

**Why separate DTO from Entity?**
- **Client sends:** `EmployeeDto` (no Id - generated server-side)
- **Database stores:** `EmployeeEntity` (with Id)
- **Prevents:** Clients from setting IDs or other protected fields

---

## ✅ Why This Architecture?

| Layer | Purpose | Benefit |
|-------|---------|---------|
| **API** | HTTP endpoints | Client communication |
| **Service** | Business logic | Orchestrates operations, validation |
| **Repository** | Data access | Database independence |
| **Domain** | Data model | Core business concepts |
| **DTO** | Data transfer | API contract, security |

**Key Principle:** Each layer has one job and doesn't leak implementation details to the layer above.

---

## 🚀 Quick Reference

**Request Path:**
```
Client → Controller → Service → Repository → Database
                       ↓
                    (DTOs)
                       ↓
                    Response
```

**Key Methods:**
- **GetAll()** - Retrieve all employees
- **GetById(id)** - Retrieve single employee
- **Create(dto)** - Add new employee
- **Update(dto)** - Modify employee
- **Delete(id)** - Remove employee

---

**Project:** Employee Management Microservices System  
**Last Updated:** March 28, 2026
