# 🏁 RACEDAY – EVENT MANAGEMENT SYSTEM

### Programming 2B | Part 3 Portfolio of Evidence

<p align="center">
  <strong>A Full-Stack Event Management Application</strong>
</p>

<p align="center">
  ASP.NET Core MVC • ASP.NET Core Web API • SQL Server • Azure Blob Storage • Docker
</p>

---

# 📑 Table of Contents

1. [Project Overview](#-1-project-overview)
2. [Project Objectives](#-2-project-objectives)
3. [System Architecture](#-3-system-architecture)
4. [Technologies Used](#-4-technologies-used)
5. [Application Features](#-5-application-features)
6. [MVC Implementation](#-6-aspnet-core-mvc-implementation)
7. [Web API Implementation](#-7-aspnet-core-web-api-implementation)
8. [Database Implementation](#-8-database-implementation)
9. [Authentication and Security](#-9-authentication-and-security)
10. [Azure Blob Storage Integration](#-10-azure-blob-storage-integration)
11. [Docker Implementation](#-11-docker-implementation)
12. [Automated Testing](#-12-automated-testing)
13. [GitHub Actions and CI/CD](#-13-github-actions-and-continuous-integration)
14. [Screenshots and Evidence](#-14-screenshots-and-implementation-evidence)
15. [Project Structure](#-15-project-structure)
16. [Installation and Setup](#-16-installation-and-setup)
17. [Conclusion](#-17-conclusion)
18. [GitHub Repository](#-18-github-repository)

---

# 📖 1. Project Overview

RaceDay is a web-based event management system developed as part of the Programming 2B Part 3 Portfolio of Evidence.

The purpose of the application is to provide a central platform where race organisers can manage sporting events, categories, participant enrolments and race results. Participants can register, log in, view their enrolled events, access their race results and manage their personal profiles.

The application was developed using **ASP.NET Core MVC** and **ASP.NET Core Web API**. The MVC application provides the user interface, while the Web API handles backend operations and communication with the database.

The project also incorporates SQL Server, Entity Framework Core, JWT authentication, Azure Blob Storage, Docker, automated testing and GitHub Actions.

Throughout the development process, different application components were implemented, integrated and tested to create a functional event management system.

# 🎯 2. Project Objectives

The main objectives of the RaceDay project were to:

- Develop a functional event management website.
- Implement an ASP.NET Core MVC frontend.
- Develop a separate ASP.NET Core Web API backend.
- Connect the MVC application to the Web API.
- Implement database operations using Entity Framework Core.
- Allow organisers to manage events, categories, enrolments and results.
- Allow participants to access their event and result information.
- Implement authentication and role-based authorisation.
- Integrate Azure Blob Storage for participant profile pictures.
- Containerise the application using Docker.
- Implement automated testing.
- Use GitHub for source code management.
- Configure GitHub Actions for continuous integration.

# 🏗️ 3. System Architecture

RaceDay follows a layered architecture that separates the frontend, backend, database and cloud storage functionality.

### Architecture Diagram

```mermaid
flowchart TD
    A["Participant / Organiser"] --> B["ASP.NET Core MVC"]
    B --> C["MVC Controllers and API Services"]
    C --> D["ASP.NET Core Web API"]
    D --> E["JWT Authentication and Authorisation"]
    D --> F["Entity Framework Core"]
    F --> G[("SQL Server Database")]
    D --> H["Azure Blob Storage"]
    H --> I["Participant Profile Pictures"]
```

### How the system works

1. A participant or organiser accesses the RaceDay website.
2. The MVC application receives the user's request.
3. MVC controllers communicate with the API through dedicated service classes.
4. The API processes the request and checks authentication and authorisation where required.
5. Entity Framework Core handles database operations.
6. SQL Server stores the application's structured information.
7. Azure Blob Storage handles profile picture storage.
8. The API returns the required information to the MVC application.
9. The MVC application displays the information to the user.

This architecture separates responsibilities between the frontend and backend, making the application easier to maintain and develop.

# 🛠️ 4. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| .NET 10 | Application framework |
| ASP.NET Core MVC | Frontend web application |
| ASP.NET Core Web API | Backend services |
| Entity Framework Core | Database operations |
| SQL Server / LocalDB | Relational database |
| JWT | User authentication |
| BCrypt | Password hashing |
| Azure Blob Storage | Cloud profile picture storage |
| Docker | Application containerisation |
| Git | Version control |
| GitHub | Source code repository |
| GitHub Actions | Continuous integration |
| Visual Studio | Development environment |

# ⭐ 5. Application Features

## 👤 Participant Functionality

Participants can:

- Register an account.
- Log into the application.
- Access their participant functionality.
- View their enrolled events.
- View their race results.
- View finishing positions and race times.
- View and update profile information.
- Upload and display a profile picture.

## 🏆 Organiser Functionality

Organisers can:

- Access the organiser dashboard.
- Create new race events.
- View existing events.
- Edit event information.
- Delete events.
- Create and manage race categories.
- View participant enrolments.
- Manage race results.

## Main Application Areas

The system separates participant and organiser functionality to make the application easier to navigate and manage.

# 💻 6. ASP.NET Core MVC Implementation

The MVC application provides the frontend of RaceDay. It is responsible for displaying information, receiving user input and communicating with the backend API.

### MVC Components

| Component | Responsibility |
|---|---|
| AccountController | Registration and login |
| HomeController | Public website pages |
| ParticipantController | Participant functionality |
| OrganiserController | Organiser functionality |
| View Models | Transfer information between controllers and views |
| Razor Views | Display application pages |
| API Services | Communicate with the Web API |
| SessionBearerTokenHandler | Pass the session authentication token |

### MVC functionality implemented

- User registration and login pages.
- Public homepage and event browsing pages.
- Participant dashboard and profile pages.
- Participant events and results pages.
- Organiser dashboard.
- Event and category management pages.
- Enrolment and results management pages.
- Profile picture display.

The MVC application uses controllers, views, models and services to separate the different responsibilities of the frontend.

# ⚙️ 7. ASP.NET Core Web API Implementation

The Web API provides the backend functionality for RaceDay.

It processes requests from the MVC application and handles the required application and database operations.

### API Controllers

| Controller | Responsibility |
|---|---|
| UsersController | User registration and user operations |
| EventsController | Event management |
| CategoriesController | Category management |
| EnrolmentsController | Participant enrolments |
| ResultsController | Race results |
| ProfilePicturesController | Profile picture functionality |

### Data Transfer Objects (DTOs)

DTOs are used to transfer information between the MVC application and API without directly exposing database entities.

Examples include:

- RegisterDTO
- LoginDTO
- EventDTO
- CategoryDTO
- EnrolmentDTO
- ResultDTO
- UserProfileDTO
- EnrolmentDetailsDTO
- ResultDetailsDTO

### Swagger Documentation

Swagger is used to document and test the API endpoints.

It provides an interface where API requests can be inspected and tested during development.

The Swagger interface is available at:

```text
http://localhost:5000/swagger
```

# 🗄️ 8. Database Implementation

RaceDay uses SQL Server together with Entity Framework Core.

Entity Framework Core manages communication between the application and the database.

### Main Database Entities

| Entity | Description |
|---|---|
| User | Stores participant and organiser information |
| Event | Stores race event details |
| Category | Stores race categories |
| Enrolment | Stores participant enrolments |
| Result | Stores participant race results |

### Database Migrations

Entity Framework Core migrations were used to create and update the database structure during development.

The project includes migrations for the main application tables and additional profile picture storage information.

The database connection is configured using application configuration and User Secrets where appropriate.

# 🔐 9. Authentication and Security

Security is an important part of the RaceDay application.

The system implements:

- JWT-based authentication.
- Password hashing and verification using BCrypt.
- Role-based access for organisers and participants.
- Protected API controllers and endpoints.
- Session-based token handling between MVC and the API.

### Password Security

BCrypt is used to hash passwords before they are stored. Password verification is performed against the stored hash rather than storing plain-text passwords.

### Role-Based Authorisation

The application separates participant and organiser functionality.

Organiser functionality is protected so that participants cannot access restricted organiser operations.

Sensitive information, including JWT signing keys and Azure credentials, must be configured securely and must never be committed to the public GitHub repository.

# ☁️ 10. Azure Blob Storage Integration

Azure Blob Storage was integrated into RaceDay to provide cloud-based storage for participant profile pictures.

Instead of relying only on a local application folder, the application uses Azure Blob Storage to store profile images.

### Azure Implementation Process

1. A participant selects a profile picture through the MVC profile page.
2. The MVC application sends the upload request to the API.
3. The API processes the profile picture request.
4. The Blob Storage service handles communication with Azure Storage.
5. The image is uploaded to the Azure Blob container.
6. The application maintains the relevant profile picture reference.
7. The participant can view the uploaded image on their profile.

### Azure Functionality Tested

- Profile picture upload.
- Profile picture display.
- Saving participant profile information.
- Retaining the profile picture after logging out and logging back in.

### Participant Profile Evidence

![RaceDay Participant Profile](Submission_Evidence/Docker_Testing/01_Participant_My_Profile.png)

**Figure 1:** Participant profile showing personal information and the uploaded profile picture.

Azure credentials are configured securely and are not included in the public source code.

# 🐳 11. Docker Implementation

Docker was used to test RaceDay in a containerised environment.

The tested setup included a SQL Server container and an MVC application container. The API ran separately on the host machine and was accessed by the MVC container through the host gateway.

### Docker Components

| Component | Configuration |
|---|---|
| SQL Server | `raceday-sql` |
| MVC application | `raceday-mvc` |
| API | Host machine, port 5000 |
| MVC website | `http://localhost:8080` |
| API Swagger | `http://localhost:5000/swagger` |

### Docker Communication

The MVC container communicates with the API through:

```text
http://host.docker.internal:5000/
```

The MVC website is accessed through:

```text
http://localhost:8080
```

### Docker Testing Performed

The following tests were performed:

- Starting the Docker containers.
- Checking container status in Docker Desktop.
- Accessing the MVC website through the browser.
- Logging in as a participant.
- Viewing participant events and results.
- Updating participant profile information.
- Accessing organiser functionality.
- Creating and managing events.
- Restarting the MVC container.
- Checking that application data remained available after the MVC container restarted.

### Docker Evidence – Initial Container State

![Docker Initial Containers](Submission_Evidence/Docker_Testing/04_Docker_Desktop_Containers_Initial.png)

**Figure 2:** Initial Docker Desktop container state.

### Docker Evidence – Running Containers

![Docker Running Containers](Submission_Evidence/Docker_Testing/05_Docker_Desktop_Containers_Running.png)

**Figure 3:** RaceDay Docker containers running.

### Additional Docker Evidence

![Docker Additional Containers](Submission_Evidence/Docker_Testing/14_Docker_Desktop_Containers_Additional.png)

**Figure 4:** Additional Docker Desktop testing evidence.

# 🧪 12. Automated Testing

Automated testing was performed using the RaceDay.Tests project.

The tests were executed using the following command:

```powershell
dotnet test RaceDay.slnx
```

### Latest Local Test Results

| Test Result | Number |
|---|---:|
| Passed | 5 |
| Failed | 0 |
| Skipped | 0 |
| Total | 5 |

### Automated Testing Evidence

![RaceDay Automated Tests](Submission_Evidence/Docker_Testing/15_Automated_Tests_Passed.png)

**Figure 5:** Automated tests showing five successful tests and zero failures.

Automated testing helps identify errors and confirms that the tested application functionality behaves as expected.

# 🔄 13. GitHub Actions and Continuous Integration

GitHub Actions was configured to automate the build and testing process.

The workflow is located at:

```text
.github/workflows/dotnet.yml
```

The workflow is triggered when code is pushed to the repository or when a pull request is created.

### Workflow Stages

1. Checkout repository.
2. Set up the .NET 10 SDK.
3. Restore dependencies.
4. Build the solution.
5. Execute automated tests.

### GitHub Actions Configuration

The workflow uses:

```yaml
dotnet-version: 10.0.x
```

The commands executed by the workflow are:

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

This provides an automated way of checking that the solution builds and that the automated tests execute.

The latest GitHub Actions result must be checked after the final project changes have been pushed.

# 📸 14. Screenshots and Implementation Evidence

This section provides visual evidence of the RaceDay application functionality.

The screenshots were captured during the implementation and testing process.

## A. Participant Functionality

### Figure 6 – Participant Profile

![Participant Profile](Submission_Evidence/Docker_Testing/01_Participant_My_Profile.png)

The participant profile displays personal information and the uploaded profile picture.

### Figure 7 – Participant Results

![Participant Results](Submission_Evidence/Docker_Testing/02_Participant_My_Results.png)

The participant can view race results, including recorded race times and finishing positions.

### Figure 8 – Participant Events

![Participant Events](Submission_Evidence/Docker_Testing/03_Participant_My_Events.png)

The participant can view events they are enrolled in.

---

## B. Docker Implementation

### Figure 9 – Initial Docker Container State

![Docker Initial State](Submission_Evidence/Docker_Testing/04_Docker_Desktop_Containers_Initial.png)

This screenshot provides evidence of the initial Docker container state.

### Figure 10 – Docker Containers Running

![Docker Running](Submission_Evidence/Docker_Testing/05_Docker_Desktop_Containers_Running.png)

This screenshot shows the RaceDay containers running in Docker Desktop.

### Figure 11 – Additional Docker Evidence

![Docker Additional](Submission_Evidence/Docker_Testing/14_Docker_Desktop_Containers_Additional.png)

Additional evidence of the Docker environment.

---

## C. Organiser Functionality

### Figure 12 – Organiser Dashboard

![Organiser Dashboard](Submission_Evidence/Docker_Testing/13_Organiser_Dashboard.png)

The organiser dashboard provides access to the event management functionality.

### Figure 13 – Event Management

![Organiser Events](Submission_Evidence/Docker_Testing/12_Organiser_Manage_Events.png)

The organiser can view and manage race events.

### Figure 14 – Create Event Form

![Create Event Form](Submission_Evidence/Docker_Testing/08_Organiser_Create_Event_Form.png)

The form allows organisers to enter information when creating a race event.

### Figure 15 – Event Created Successfully

![Event Created](Submission_Evidence/Docker_Testing/07_Organiser_Event_Created_Successfully.png)

Evidence showing successful event creation.

### Figure 16 – Event Deleted Successfully

![Event Deleted](Submission_Evidence/Docker_Testing/06_Organiser_Event_Deleted_Successfully.png)

Evidence showing successful event deletion.

### Figure 17 – Category Management

![Category Management](Submission_Evidence/Docker_Testing/11_Organiser_Manage_Categories.png)

The organiser can manage race categories.

### Figure 18 – Participant Enrolments

![Organiser Enrolments](Submission_Evidence/Docker_Testing/09_Organiser_View_Enrolments.png)

The organiser can view participant enrolments.

### Figure 19 – Race Results Management

![Results Management](Submission_Evidence/Docker_Testing/10_Organiser_Manage_Results.png)

The organiser can manage race results.

---

## D. Automated Testing Evidence

### Figure 20 – Automated Tests Passed

![Automated Tests](Submission_Evidence/Docker_Testing/15_Automated_Tests_Passed.png)

The automated test screenshot shows five tests passed and zero failures.

---

# 📂 15. Project Structure

The main solution structure is organised as follows:

```text
RaceDay/
│
├── RaceDay.Api/
│   ├── Controllers/
│   ├── DTOs/
│   ├── Data/
│   ├── Migrations/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   └── RaceDay.Api.csproj
│
├── RaceDay.Contracts/
│   ├── DTOs/
│   ├── Models/
│   └── RaceDay.Contracts.csproj
│
├── RaceDay.Mvc/
│   ├── Controllers/
│   ├── Models/
│   ├── Properties/
│   ├── Service/
│   ├── Views/
│   ├── wwwroot/
│   ├── Dockerfile
│   ├── Program.cs
│   └── RaceDay.Mvc.csproj
│
├── RaceDay.Tests/
│
├── Submission_Evidence/
│   └── Docker_Testing/
│       ├── 01_Participant_My_Profile.png
│       ├── 02_Participant_My_Results.png
│       ├── 03_Participant_My_Events.png
│       ├── 04_Docker_Desktop_Containers_Initial.png
│       ├── 05_Docker_Desktop_Containers_Running.png
│       ├── 06_Organiser_Event_Deleted_Successfully.png
│       ├── 07_Organiser_Event_Created_Successfully.png
│       ├── 08_Organiser_Create_Event_Form.png
│       ├── 09_Organiser_View_Enrolments.png
│       ├── 10_Organiser_Manage_Results.png
│       ├── 11_Organiser_Manage_Categories.png
│       ├── 12_Organiser_Manage_Events.png
│       ├── 13_Organiser_Dashboard.png
│       ├── 14_Docker_Desktop_Containers_Additional.png
│       └── 15_Automated_Tests_Passed.png
│
├── .github/
│   └── workflows/
│       └── dotnet.yml
│
├── RaceDay.slnx
└── README.md
```

# 🚀 16. Installation and Setup

## Requirements

Before running the application, ensure that the following are installed:

- Visual Studio with .NET 10 support.
- .NET 10 SDK.
- SQL Server or SQL Server LocalDB.
- Git.
- Docker Desktop for the containerised setup.

## Step 1 – Clone the Repository

```bash
git clone https://github.com/Kcleophas/RaceDayApi.git
```

## Step 2 – Open the Solution

Open the following solution file in Visual Studio:

```text
RaceDay.slnx
```

## Step 3 – Restore Dependencies

Run from the solution directory:

```bash
dotnet restore
```

## Step 4 – Configure Application Settings

Configure the database connection and required authentication and Azure settings using the appropriate secure configuration method.

Sensitive credentials must not be committed to GitHub.

## Step 5 – Apply Database Migrations

Using the Visual Studio Package Manager Console:

```powershell
Update-Database
```

## Step 6 – Start the API

From the solution directory:

```powershell
dotnet run --project RaceDay.Api/RaceDay.Api.csproj --urls http://0.0.0.0:5000
```

Swagger can be accessed at:

```text
http://localhost:5000/swagger
```

## Step 7 – Start the MVC Application

Open another terminal and run:

```powershell
dotnet run --project RaceDay.Mvc/RaceDay.Mvc.csproj
```

Open the local MVC address displayed in the terminal.

## Step 8 – Run Automated Tests

```powershell
dotnet test RaceDay.slnx
```

---

# 🏆 17. Conclusion

The RaceDay Event Management System demonstrates the development of a full-stack web application using ASP.NET Core MVC and ASP.NET Core Web API.

The project combines frontend development, backend services, database management, authentication, cloud storage, containerisation and automated testing.

The implementation provides separate functionality for participants and organisers, allowing users to manage events, enrolments, results and personal profiles.

Azure Blob Storage provides cloud-based profile picture storage, while Docker supports testing in a containerised environment. GitHub Actions provides an automated build and testing process.

Through the development of RaceDay, practical experience was gained in integrating different technologies, managing application components, implementing security and testing a complete web application.

Overall, RaceDay demonstrates the practical application of software development concepts through the creation of a structured event management system.

---

# 🔗 18. GitHub Repository

**Project Name:** RaceDay – Event Management System

**Module:** Programming 2B – Part 3 Portfolio of Evidence

**GitHub Repository:**

https://github.com/Kcleophas/RaceDayApi

---

<p align="center">
  <strong>RaceDay – Manage Events. Track Results. Connect Participants.</strong>
</p>