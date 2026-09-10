POC Summary — FieldPro

POC objective
- Validate core mobile scenario for FieldPro: secure authentication, reliable data sync (online + offline), and role-based UI flows.

Scope
- Mobile client: .NET MAUI app (Android + iOS + Windows)
- Backend API: ASP.NET Core minimal API or Web API
- Data store: Azure SQL (primary), local SQLite on device
- Auth: Entra ID B2C (recommended) or custom JWT-based auth for POC
- Storage: Azure Blob Storage for media attachments
- Sync: Background sync worker on client, conflict resolution: last-writer-wins (POC)

Success criteria
- End-to-end authentication and authorization working
- Create/read/update of core entity (WorkOrder) from mobile and visible in backend DB
- Offline create and subsequent background sync when online
- Basic telemetry/logging available for troubleshooting

Assumptions
- Existing REST API or willingness to scaffold an API during POC
- Dev/test Azure subscription available for POC resources
- Minimal security hardening acceptable for POC (no production secrets in code)

High-level architecture (textual)

[Mobile (MAUI)]
- UI, local SQLite with change tracking, background sync service, HTTP client with retry
- Auth handled by MSAL / Entra B2C or token endpoint

	  +----------------+
	  | Mobile (.NET   |
	  | MAUI)          |
	  | - Local SQLite |
	  | - Sync worker  |
	  +--------+-------+
			   |
			   | HTTPS (JWT/Bearer)
			   v
+--------------+------------------+
| API (ASP.NET Core)              |
| - Auth validation (JWT / B2C)   |
| - Business logic                 |
| - Sync endpoints                 |
+--------------+------------------+
			   |
		+------+-------+
		|              |
		v              v
  Azure SQL         Azure Blob
  (backend store)   (attachments)

Components
- Mobile: MAUI app, SQLite, background sync scheduler, MSAL for auth
- API: ASP.NET Core, EF Core (Azure SQL), endpoints for WorkOrders and attachments
- Infra: Azure App Service or Container Apps, Azure SQL Database, Blob Storage, Application Insights

Data flow
1. User authenticates on device (MSAL / token) and receives JWT
2. Device calls API to pull initial data and push local changes
3. API persists to Azure SQL and stores large attachments to Blob Storage
4. Device syncs in background, resolves conflicts via last-writer-wins (POC)

Minimal deployment
- Resource group with App Service, Azure SQL, Blob Storage, Application Insights
- CI: GitHub Actions to build API and publish to App Service; MAUI builds via dev machine or App Center for device builds

POC timeline (suggested)
- Day 0: Plan, scaffold API and MAUI shell
- Day 1: Implement auth flow + basic API endpoints
- Day 2: Local DB + sync prototype (CRUD roundtrip)
- Day 3: Attachments + telemetry + demo

Next steps / Deliverables
- Prototype MAUI app with auth and a WorkOrder screen
- Minimal API with WorkOrder endpoints and DB migrations
- Deployment scripts (ARM/Bicep or Terraform) for POC resources

How to generate PDF from this file
- If you have pandoc installed (Windows PowerShell):
  pandoc "FieldPro/Docs/POC_and_HighLevel_Architecture.md" -o "FieldPro/Docs/POC_and_HighLevel_Architecture.pdf"
- Or open the markdown in Visual Studio / VS Code and print to PDF.

If you want, I can convert this to a PDF and add a simple diagram image in the repo; tell me to create the PDF file in the workspace and whether to include a PNG architecture image.