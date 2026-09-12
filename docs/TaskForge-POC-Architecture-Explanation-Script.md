# TaskForge POC Architecture PDF Explanation Script

## Opening

**Say:**

> This document explains the TaskForge proof of concept, the current implementation scope, and the high-level architecture behind the application.

> TaskForge is built with .NET MAUI, Shell navigation, CommunityToolkit.Mvvm, dependency injection, and local SQLite storage.

## Page 1: POC Overview

**Point to:** The title and technology summary.

**Say:**

> The POC is a local task management application. It demonstrates the main user experience without requiring a backend server.

> The application includes demo-user login, dashboard counts, task management, projects, notifications, documents, and device capability demonstrations.

> Because this is a POC, data is stored locally in the application database. It is not synchronized between devices.

## Page 2: Main User Flow

**Point to:** The flow from user selection to logout.

**Say:**

> The user first selects a demo user. That selected user is stored in the in-memory application session.

> The dashboard then loads task counts according to the user's role. Managers, team leads, and administrators can see all tasks. Employees see only the tasks assigned to them.

> From the dashboard, the user can open the complete task board, open documents, or access device capabilities.

> Logout clears the current session and returns the user to the Login page.

## Page 3: High-Level Architecture Diagram

**Point to:** The architecture diagram from top to bottom.

### Presentation Layer

**Say:**

> The top layer is the XAML presentation layer. These pages define what the user sees and how controls are arranged.

> LoginPage handles demo-user selection. DashboardPage shows role-aware counts. TasksPage displays the task board and statuses. DocumentsPage opens the project PDF. DeviceCapabilitiesPage exposes camera, GPS, and accelerometer actions.

### MVVM Layer

**Say:**

> The second layer is MVVM. ViewModels contain screen state and commands, keeping business behavior out of the XAML code-behind.

> CommunityToolkit.Mvvm generates observable properties and commands using attributes such as `[ObservableProperty]` and `[RelayCommand]`.

> For example, DashboardViewModel loads task counts and exposes navigation commands, while TasksViewModel handles task creation and status updates.

### Services Layer

**Say:**

> The service layer contains the reusable application behavior.

> AuthService finds the selected demo user. TaskForgeDatabase handles SQLite reads and writes. AppSession stores the current user during the session. DeviceCapabilitiesService wraps camera, GPS, accelerometer, and permission APIs.

### Storage and Platform Layer

**Say:**

> The bottom layer contains the local database and operating-system APIs.

> Task data is stored in `taskforge.db3` under the platform application-data directory. PDFs and captured photos are stored in app storage. MAUI Essentials provides the platform APIs for permissions, camera, location, sensors, and opening files.

## Page 4: Task and Document Design

**Point to:** The feature behavior table.

**Say:**

> Dashboard counts and task-board rows are based on the same SQLite task data.

> Administrators, project managers, and team leads load all task rows. Employees load only rows whose AssignedUserId matches the current user.

> Each task row shows the title, description, due date, assigned user, and current status. The status picker writes changes back to SQLite.

> The Documents page uses a bundled project brief PDF. When the user selects Open PDF, the application copies the file into app storage and opens it with the platform file launcher.

## Page 5: Device Capability UI

**Point to:** The device capability table.

**Say:**

> Device capability code is now exposed through a dedicated UI screen.

> The Camera action requests permission, captures a photo, and saves it locally.

> The GPS action requests location permission and displays latitude and longitude.

> The Accelerometer action starts monitoring and displays live X, Y, and Z readings. Monitoring stops when the user selects Stop or leaves the page.

> Platform-specific Entry handler mappings are registered in MauiProgram for Android and iOS customization.

## Page 6: Acceptance Criteria

**Say:**

> These criteria describe the expected behavior of the POC.

> The important checks are role-aware task counts, a complete task board without an accidental hidden filter, visible task statuses, working PDF opening, and device permission/result feedback.

> The application also needs to build successfully for the Windows target.

## Page 7: Production Next Steps

**Say:**

> The POC proves the client-side architecture, but production would require a backend identity service and API-backed task storage.

> Additional production work would include token authentication, server-side authorization, audit logging, offline synchronization, conflict resolution, and automated tests.

> Captured photos and locations should eventually be stored against task records instead of only being saved locally.

## Architecture Summary

**Say:**

> In summary, TaskForge follows a layered MAUI architecture: XAML pages bind to ViewModels, ViewModels call injected services, and services work with SQLite or platform APIs.

> This keeps the UI separate from data access and makes the application easier to test, extend, and connect to a backend later.

## Closing

**Say:**

> The current POC demonstrates the complete local workflow for TaskForge: user selection, role-aware task visibility, task status management, document access, and device capability interaction.

> The next architectural step is to preserve this client structure while replacing local-only authentication and data access with secure backend services.
