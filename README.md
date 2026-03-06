# Ticket System

Full Stack ticket management application built with ASP.NET Core and Vue.js following Clean Architecture principles.

This project allows users to create, manage, assign, and track support tickets through a modern web interface connected to a REST API.

One of the key features of the system is an integrated chat that allows users to communicate directly with operators and administrators within the platform.

---

## Tech Stack

### Backend

* C#
* ASP.NET Core
* Entity Framework Core
* SQL Database
* JWT Authentication

### Frontend

* Vue.js
* JavaScript
* HTML
* CSS
* TailwindCSS / Bootstrap

### Testing

* Unit Testing
* Integration Testing

### Other Tools

* Git
* REST API
* Clean Architecture

---

## Architecture

The backend follows Clean Architecture principles to ensure scalability, maintainability, and proper separation of concerns.

Layers used in the backend:

**Domain**
Contains core business entities and business rules.

**Application**
Contains use cases, interfaces, and application logic.

**Infrastructure**
Handles database access, repositories, and external services.

**API (Presentation Layer)**
Exposes REST endpoints that are consumed by the frontend application.

---

## Key Feature

**Integrated Support Chat**

The system includes a built-in chat that allows users to communicate directly with operators and administrators within the platform.
This enables faster support and real-time interaction without leaving the ticket management system.

---

## Features

* User authentication using JWT
* Role-based authorization
* Create support tickets
* Assign tickets to users
* Update and manage ticket status
* Integrated chat between users, operators and administrators
* Ticket management dashboard
* RESTful API
* Frontend interface built with Vue.js
* Automated testing

---

## Testing

The project includes automated tests to ensure reliability and maintainability.

Tests cover:

* Application services
* Business logic
* API endpoints

Tests can be executed with:

dotnet test

---

## Project Structure

```text id="u3aj1o"
TicketSystem
│
├── backend
│   ├── Domain
│   ├── Application
│   ├── Infrastructure
│   └── API
│
└── frontend
    ├── components
    ├── pages
    └── services
```

---

## Installation

### 1. Clone the repository

git clone https://github.com/julichiosso/TicketSystem.API.git

### 2. Configure the database

Update the database connection string in:

appsettings.json

### 3. Run database migrations

dotnet ef database update

### 4. Run the backend

dotnet run

### 5. Run the frontend

Navigate to the frontend folder and install dependencies:

npm install
npm run dev

---

## API Documentation

API endpoints can be explored using Swagger.

Available at:

/swagger

---

## Author

Julian Chiosso
Full Stack Web Developer

GitHub: https://github.com/julichiosso
