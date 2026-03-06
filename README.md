# Ticket System

Full Stack ticket management application built with ASP.NET Core and Vue.js following Clean Architecture principles.

This project allows users to create, manage, and track support tickets through a modern web interface connected to a REST API.

---

## Tech Stack

### Backend
- C#
- ASP.NET Core
- Entity Framework Core
- SQL Database
- JWT Authentication

### Frontend
- Vue.js
- JavaScript
- HTML
- CSS
- TailwindCSS / Bootstrap

### Other Tools
- Git
- REST API
- Clean Architecture

---

## Architecture

The backend follows Clean Architecture principles to ensure scalability, maintainability, and separation of concerns.

Layers used in the backend:

- **Domain**  
  Contains core business entities and rules.

- **Application**  
  Contains use cases, interfaces, and application logic.

- **Infrastructure**  
  Handles database access and external services.

- **API (Presentation Layer)**  
  Exposes REST endpoints used by the frontend.

---

## Features

- User authentication with JWT
- Create support tickets
- Update ticket status
- View and manage tickets
- RESTful API
- Frontend interface built with Vue.js

---

## Project Structure
TicketSystem
│
├── backend
│ ├── Domain
│ ├── Application
│ ├── Infrastructure
│ └── API
│
└── frontend
├── components
├── pages
└── services

