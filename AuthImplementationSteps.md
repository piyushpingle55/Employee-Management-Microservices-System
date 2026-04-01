# JWT Authentication Implementation Guide

Complete end-to-end JWT authentication implementation for Employee Management Microservices System.

## Quick Overview

- **AuthService**: Issues JWT tokens after validating user credentials
- **EmployeeService**: Validates incoming JWT tokens and protects endpoints
- **Shared Config**: Both services use same Issuer, Audience, and Key

---

## PHASE 1: AuthService Setup (Token Issuer)

### Step 1: Create User Domain Entity
**File**: `src/Services/AuthService/Auth.Domain/Entities/User.cs`
- Create `User` class with: Id (Guid), Email (string), PasswordHash (string), Role (string), CreatedAt (DateTime)
- **Why**: Represents user in domain layer, never store plain passwords

### Step 2: Create Database Context
**File**: `src/Services/AuthService/Auth.Infrastructure/Data/AuthDbContext.cs`
- Create `AuthDbContext` with `DbSet<User> Users`
- Configure unique index on Email field
- **Why**: Maps user domain to database table, ensures email uniqueness

### Step 3: Create User Repository
**File**: `src/Services/AuthService/Auth.Infrastructure/Repositories/UserRepository.cs`
- Create `GetByEmailAsync(email)` method
- Create `AddAsync(user)` method for registration
- **Why**: Centralizes all database queries for users

### Step 4: Create DTOs
**Files**: 
- `LoginRequest.cs`: Email + Password
- `LoginResponse.cs`: Token + Expiration
- `RegisterRequest.cs`: Email + Password + Role
- **Why**: Decouple API contracts from domain models

### Step 5: Create AuthService with JWT Generation
**File**: `src/Services/AuthService/Auth.Application/Services/AuthService.cs`
- Implement `LoginAsync()`: Find user, verify password hash, generate JWT token
- Implement `RegisterAsync()`: Create new user, hash password, save to DB
- Implement `GenerateJwtToken()`: Create JWT with claims, sign with secret key, set 1-hour expiry
- **Why**: Core authentication logic, password hashing with PBKDF2, JWT creation

### Step 6: Create AuthService Interface
**File**: `src/Services/AuthService/Auth.Application/Interfaces/IAuthService.cs`
- Define `LoginAsync(LoginRequest)` and `RegisterAsync(RegisterRequest)` methods
- **Why**: Enable dependency injection and testability

### Step 7: Create Auth Controller
**File**: `src/Services/AuthService/Auth.API/Controllers/AuthController.cs`
- Create `/api/auth/register` POST endpoint
- Create `/api/auth/login` POST endpoint
- Add error handling (return 400 for duplicate users, 401 for invalid credentials)
- **Why**: Expose authentication endpoints to clients

### Step 8: Configure AuthService Program.cs
**File**: `src/Services/AuthService/Auth.API/Program.cs`
- Register `AuthDbContext` with SQL Server connection
- Register `UserRepository` and `IAuthService` as scoped services
- Add Swagger support
- Add controllers and mapping
- **Why**: DI setup and middleware configuration

### Step 9: Update AuthService Configuration
**File**: `src/Services/AuthService/Auth.API/appsettings.json`
```json
"ConnectionStrings": { "AuthDb": "Server=localhost;Database=AuthDataBase;..." }
"Jwt": {
  "Issuer": "EmployeeManagementSystem",
  "Audience": "EmployeeManagementClients",
  "Key": "ThisIsAVeryStrongSecretKeyForJwtToken123!"
}
```
- **Why**: Configuration values must match between AuthService and EmployeeService

---

## PHASE 2: EmployeeService Setup (Protected Resource)

### Step 10: Add JWT Authentication Middleware
**File**: `src/Services/EmployeeService/Employee.API/Program.cs`
- Add `using Microsoft.AspNetCore.Authentication.JwtBearer`
- Register JWT Bearer authentication with token validation parameters
- Set `ValidateIssuer = true`, `ValidateAudience = true`, `ValidateLifetime = true`, `ValidateIssuerSigningKey = true`
- Configure `ValidIssuer`, `ValidAudience`, `IssuerSigningKey` from appsettings
- Add `builder.Services.AddAuthorization()`
- **Why**: Validates incoming JWTs before reaching controllers

### Step 11: Apply [Authorize] Attribute to Controller
**File**: `src/Services/EmployeeService/Employee.API/Controllers/EmployeeController.cs`
- Add `using Microsoft.AspNetCore.Authorization`
- Add `[Authorize]` attribute to class (all endpoints require JWT)
- Add `[Authorize(Roles = "Admin")]` to Create, Update, Delete endpoints
- **Why**: Enforces authentication + role-based authorization

### Step 12: Update EmployeeService Configuration
**File**: `src/Services/EmployeeService/Employee.API/appsettings.json`
```json
"Jwt": {
  "Issuer": "EmployeeManagementSystem",
  "Audience": "EmployeeManagementClients",
  "Key": "ThisIsAVeryStrongSecretKeyForJwtToken123!"
}
```
- **Why**: Must match AuthService exactly for token validation to succeed

### Step 13: Add Required Packages
- `Microsoft.AspNetCore.Authentication.JwtBearer` to both API projects
- `System.IdentityModel.Tokens.Jwt` to Auth.Application
- **Why**: NuGet packages provide JWT functionality

---

## Testing Checklist

- [ ] Create AuthDataBase with Users table
- [ ] `dotnet build` succeeds for entire solution
- [ ] Run AuthService: `dotnet run` from Auth.API folder
- [ ] Register user via `/api/auth/register` (Swagger)
- [ ] Login and get JWT token via `/api/auth/login`
- [ ] Copy token and authorize in EmployeeService Swagger (click Authorize button, paste `Bearer <token>`)
- [ ] Test GET `/api/employee` - should return 200 OK
- [ ] Test POST `/api/employee` with User token - should return 403 Forbidden (Admin only)
- [ ] Register Admin user, login, copy Admin token, authorize in Swagger, test POST - should return 200 OK
- [ ] Test without authorization - should return 401 Unauthorized

---

## Quick Commands

```powershell
# Build entire solution
dotnet build

# Run AuthService
cd src\Services\AuthService\Auth.API
dotnet run

# Run EmployeeService (new terminal)
cd src\Services\EmployeeService\Employee.API
dotnet run
```

---

## Security Features Implemented

✅ Passwords hashed with PBKDF2 + salt  
✅ JWT signed with HMAC-SHA256 (tamper-proof)  
✅ Token expires in 1 hour  
✅ Issuer & Audience validation  
✅ Role-based authorization  
✅ Email uniqueness enforced  
✅ No plain text passwords in DB  

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Invalid credentials" on login | Verify user exists in DB, check email casing (lowercased) |
| 401 in EmployeeService | Verify JWT config matches in both services (Issuer, Audience, Key) |
| Token validation failed | Rebuild both projects, check appsettings.json for typos |
| `[Authorize]` not found | Add `using Microsoft.AspNetCore.Authorization;` |
| Database connection error | Verify SQL Server running, connection string correct |

---

## Production Checklist

- [ ] Use HTTPS everywhere (not just dev)
- [ ] Move secrets to Key Vault (Azure/AWS)
- [ ] Add rate limiting on `/login` endpoint
- [ ] Implement refresh tokens for longer sessions
- [ ] Log all authentication failures
- [ ] Rotate JWT signing key periodically
- [ ] Test with penetration testing
