# TaskForge Demo Script

## Demo Goal

Demonstrate the TaskForge POC as a local .NET MAUI task management application with role-aware task visibility, SQLite data, MVVM screens, PDF documents, and device capability access.

## Before the Demo

1. Start the TaskForge application.
2. Confirm the app opens on the Login screen.
3. Use a Windows machine with the TaskForge database initialized.
4. For camera, GPS, or accelerometer demonstrations, use a device or emulator that supports those capabilities.

## 1. Login with a Demo User

**Say:**

> TaskForge uses a simple demo-user login for this proof of concept. The selected user controls the task visibility and permissions shown in the application.

**Action:**

1. Open the user picker.
2. Select `Ali Patel - Project Manager`.
3. Select **Login**.

**Expected result:**

- The Dashboard opens.
- The user name is shown as Ali Patel.
- The access level is shown as Project Manager.
- Ali can see project-level task counts.

## 2. Demonstrate Dashboard Counts

**Say:**

> The dashboard counts come from the local SQLite database. Managers, team leads, and administrators can see all task records, while employees see only tasks assigned to them.

**Action:**

1. Review the In Progress count.
2. Review the Not Started count.
3. Review the Completed count.
4. Select **Open tasks**.

**Expected result:**

- The counts are loaded from real `TaskItem` records.
- The task board opens without a hidden status filter.
- All tasks permitted for the selected role are visible.

## 3. Demonstrate Task Status and Assignment

**Say:**

> Each task displays its current status and assigned user. Status changes are written back to SQLite.

**Action:**

1. Point out the task title and description.
2. Point out the `Assigned to:` label.
3. Point out the visible status badge.
4. Change one task using the status picker.
5. Return to the Dashboard.

**Expected result:**

- The task row shows the assigned user name.
- The status changes in the task board.
- The dashboard count changes after the page reloads.

## 4. Demonstrate Employee Visibility

**Say:**

> The same application applies different task visibility rules based on the selected user's role.

**Action:**

1. Log out.
2. Select `Akshit Kumar - Software Engineer`.
3. Select **Login**.
4. Open the Dashboard and task board.

**Expected result:**

- Akshit sees only tasks where `AssignedUserId` matches Akshit's user ID.
- Tasks assigned to other users are not included in Akshit's counts or task list.

## 5. Demonstrate Documents and PDF Opening

**Say:**

> Documents are stored as local metadata and the project brief PDF is bundled with the application. The app copies it to app storage and opens it with the platform's default PDF viewer.

**Action:**

1. Open **Documents** from the Shell menu or Dashboard.
2. Locate `Project brief`.
3. Select **Open PDF**.

**Expected result:**

- The bundled `project-brief.pdf` is copied to app storage if needed.
- The system PDF viewer opens the document.
- The document displays the TaskForge project brief.

## 6. Demonstrate Device Capabilities

Open **Device Capabilities** from the Shell menu.

### Camera

**Say:**

> Camera permission is requested only when the camera action is used.

**Action:**

1. Select **Capture photo**.
2. Accept permission if prompted.
3. Capture or select the photo.

**Expected result:**

- The photo is saved in the application data directory.
- The screen shows the saved filename or an unavailable/cancelled message.

### GPS

**Say:**

> Location access is requested at runtime, and the current coordinates are shown only after permission is granted.

**Action:**

1. Select **Get current location**.
2. Accept permission if prompted.

**Expected result:**

- Latitude and longitude are displayed when location is available.
- A clear unavailable or denied message appears otherwise.

### Accelerometer

**Say:**

> The accelerometer is monitored only while the user explicitly starts it, and monitoring stops when the page closes.

**Action:**

1. Select **Start**.
2. Move the device or emulator.
3. Point out the X, Y, and Z values.
4. Select **Stop**.

**Expected result:**

- Live acceleration values appear.
- The Stop button stops monitoring.
- Leaving the page also stops monitoring.

## 7. Demonstrate MVVM and Architecture

**Say:**

> The UI is written in XAML, while screen behavior is implemented in ViewModels. CommunityToolkit.Mvvm generates observable properties and commands, and services are registered through dependency injection in MauiProgram.

**Show in the code:**

- `LoginPage.xaml` and `LoginViewModel.cs`
- `DashboardPage.xaml` and `DashboardViewModel.cs`
- `TasksPage.xaml` and `TasksViewModel.cs`
- `DeviceCapabilitiesPage.xaml` and `DeviceCapabilitiesViewModel.cs`
- `TaskForgeDatabase.cs`
- `MauiProgram.cs`

## 8. Logout

**Say:**

> Logout clears the current in-memory session and returns the user to the login screen.

**Action:**

1. Open the Shell menu.
2. Select **Logout**.

**Expected result:**

- The current user session is cleared.
- The Login screen opens.
- The authenticated flyout is no longer available on the Login page.

## Closing Statement

> This POC demonstrates the core TaskForge workflow: role-aware task management, local SQLite persistence, MVVM-based UI behavior, document opening, and platform capability integration. The next production step would be connecting the app to a backend identity and task API.

## Key Files

- `FieldPro/MauiProgram.cs`
- `FieldPro/AppShell.xaml`
- `FieldPro/Data/TaskForgeDatabase.cs`
- `FieldPro/ViewModels/DashboardViewModel.cs`
- `FieldPro/ViewModels/TasksViewModel.cs`
- `FieldPro/ViewModels/DeviceCapabilitiesViewModel.cs`
- `FieldPro/Views/DocumentsPage.xaml.cs`
- `FieldPro/Resources/Raw/project-brief.pdf`
