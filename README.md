# 🥋 IAMuayThai API

RESTful API for managing Muay Thai academies. This system provides control over students, teachers, class schedules, check-ins, and progression tracking.

---

## 📌 About the Project

**IAMuayThai API** is a backend application built using **ASP.NET Core** following **Clean Architecture** and **Domain-Driven Design (DDD-light)** principles. It powers a complete management platform for Muay Thai schools.

The system supports two user roles:

- 👨‍🏫 **Teacher**
- 🧑‍🎓 **Student**

---

## ✨ Features

### ✅ Common
- Secure login with role-based authentication (JWT)
- Class check-in system
- View personal training schedule
- Access training and attendance history
- Track progression toward the next Kruang level

### 👨‍🏫 Teacher Features
- List and manage students
- Register class agendas
- View students eligible for level progression

---

## 🧱 Architecture

The project follows the **Clean Architecture** approach and is split into multiple layers for scalability and maintainability:

```bash
IAMUAYTHAI-API/
├── src/
│   ├── IAMUAYTHAI.API/                      # Presentation layer (Controllers, Middlewares)
│   ├── IAMUAYTHAI.Application/              # Application services and use cases
│   ├── IAMUAYTHAI.Application.Abstractions/ # Interfaces for dependency inversion
│   ├── IAMUAYTHAI.Domain/                   # Domain entities and aggregates
│   └── IAMUAYTHAI.Infra/                    # Infrastructure layer (EF Core, Migrations)
└── test/
    └── IAMUAYTHAI.Application.Test/         # Unit and integration tests
```

---

## 🛠️ Tech Stack

| Layer           | Technology                        |
|-----------------|------------------------------------|
| Backend         | ASP.NET Core Web API              |
| Database        | SQL Server + Entity Framework Core |
| ORM             | Entity Framework Core             |
| Authentication  | JWT Bearer                        |
| Patterns        | Clean Architecture, DDD, SOLID    |
| Testing         | xUnit                             |
| Migrations      | EF Core CLI                       |

---

## 🔐 Authentication

The API uses **JWT** for authentication. Each protected endpoint requires a `Bearer` token in the request header.


---

## 🧪 Testing

To run all unit and integration tests:

```bash
dotnet test
