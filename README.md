# Employee Management Microservices System

## 🏗️ Overview

This solution implements a production-grade microservices architecture using .NET, following Clean Architecture and Domain-Driven Design principles.

The system consists of independently deployable services:

* Auth Service (JWT Authentication & Authorization)
* Employee Service
* Department Service
* Payroll Service
* API Gateway (central entry point)

---

## 🧩 Architecture Style

* Microservices Architecture
* Clean Architecture (per service)
* Database per Service
* API Gateway Pattern
* JWT-based centralized authentication

---

## 📦 Services Description

### 1. Auth Service

Handles:

* User registration
* Login
* JWT token generation
* Role-based authorization

Database: AuthDB

---

### 2. Employee Service

Handles:

* Employee CRUD operations
* Employee profile management

Database: EmployeeDB

---

### 3. Department Service

Handles:

* Department creation and management
* Department metadata

Database: DepartmentDB

---

### 4. Payroll Service

Handles:

* Salary management
* Payslip generation
* Payroll processing

Database: PayrollDB

---

### 5. API Gateway

Responsibilities:

* Central routing to microservices
* JWT token validation
* Request aggregation (future)
* Logging & rate limiting (future)

---

## 🔐 Authentication Flow

1. Client authenticates via Auth Service
2. JWT token is issued
3. Token is passed to API Gateway
4. Gateway validates token and forwards request to respective service

---

## 🧱 Solution Structure

```
src/
 ├── Services/
 │   ├── AuthService/
 │   ├── EmployeeService/
 │   ├── DepartmentService/
 │   └── PayrollService/
 │
 ├── ApiGateway/
 └── BuildingBlocks/
```

Each microservice contains:

* API Layer (Controllers)
* Application Layer (Business Logic)
* Domain Layer (Entities & Core Rules)
* Infrastructure Layer (EF Core, DbContext)

---

## 🛠️ Technology Stack

* .NET 9 Web API
* Entity Framework Core
* SQL Server
* JWT Bearer Authentication
* YARP API Gateway
* Clean Architecture + DDD

---

## 🚀 Future Enhancements

* Docker & Kubernetes deployment
* Event-driven communication (RabbitMQ)
* Distributed caching (Redis)
* Centralized logging (ELK / Seq)

---

## 📌 Design Principles

* Loose coupling between services
* Independent database per service
* Single Responsibility Principle per service
* Secure communication via JWT tokens
* Clean separation of concerns using layered architecture
