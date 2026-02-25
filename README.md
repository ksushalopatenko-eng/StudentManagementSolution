# Student Management System

A full-stack .NET project consisting of:
- **StudentManagementApi** — ASP.NET Core Web API (REST) with Swagger documentation
- **StudentManagementWeb** — ASP.NET Core Razor Pages UI that communicates with the API via `HttpClient`

The system manages **students**, **courses**, and **enrollments** (many-to-many), demonstrating a realistic CRUD workflow and a clean project structure suitable for a portfolio.

---

## Features

### Students
- Create / Read / Update / Delete students
- Search by student name (UI)

### Courses
- Create / Read / Update / Delete courses
- Search by course title (UI)
- Quick navigation to view students enrolled in a course

### Enrollments (Many-to-Many)
- Enroll a student into a course
- View:
  - courses of a student
  - students of a course
- Enrollment table view (student ↔ course)
- Remove enrollment

### Home Page
- Landing-style home page
- Quick navigation buttons (Students / Courses / Enrollments)
- Live statistics (Students / Courses / Enrollments) via API `/stats`

---

## Tech Stack

**Backend (API)**
- C# / .NET (ASP.NET Core Web API)
- Entity Framework Core + SQLite
- AutoMapper
- Swagger (OpenAPI)
- Global exception handling middleware

**Frontend (Web UI)**
- ASP.NET Core Razor Pages
- Bootstrap (default Razor template styling)
- `HttpClientFactory` for API communication

---

## Architecture Overview

### StudentManagementApi
Typical layered structure:
- `Models/` — entities (`Student`, `Course`, `Enrollment`)
- `Data/` — `AppDbContext` (EF Core)
- `Services/` — business logic (`UniversityService`)
- `Controllers/` — REST endpoints
- `Middleware/` — global exception middleware
- `Mapping/` — AutoMapper profiles

### StudentManagementWeb
- Razor Pages for UI (`Pages/Students`, `Pages/Courses`, `Pages/Enrollments`)
- HTTP calls to API via named `HttpClient` `"StudentApi"` (BaseUrl in `appsettings.json`)

---

## API Endpoints

### Students
- `GET /students`
- `GET /students/{id}`
- `POST /students`
- `PUT /students/{id}`
- `DELETE /students/{id}`

### Courses
- `GET /courses`
- `GET /courses/{id}`
- `POST /courses`
- `PUT /courses/{id}`
- `DELETE /courses/{id}`

### Enrollments
- `POST /enrollments`
- `GET /enrollments` (enrollment table rows)
- `GET /enrollments/students/{studentId}/courses`
- `GET /enrollments/courses/{courseId}/students`
- `DELETE /enrollments/students/{studentId}/courses/{courseId}`

### Stats
- `GET /stats` (students/courses/enrollments counters)

### Health
- `GET /health`

Swagger UI is available at:
- `http://localhost:<API_PORT>/swagger`

---

## Getting Started

### Prerequisites
- .NET SDK installed (recommended: .NET 8 or later)
- (Optional) Git

---

## Run the Backend (API)

```bash
cd StudentManagementApi
dotnet run
Open Swagger:

http://localhost:<5012>/swagger

The API uses SQLite (student.db) and applies EF Core migrations automatically on startup.

Run the Frontend (Web UI)

In a separate terminal:

cd StudentManagementWeb
dotnet run

Open the website:

http://localhost:<5062>

## Configuration

API Base URL (Web → API)

Update the API Base URL in:

StudentManagementWeb/appsettings.json

Example:

{
  "Api": {
    "BaseUrl": "http://localhost:5012"
  }
}

Make sure the port matches the one displayed by the API on startup:
Now listening on: http://localhost:XXXX

---

Created as a portfolio project to demonstrate full-stack .NET skills (Web API + Razor Pages + EF Core + SQLite).
