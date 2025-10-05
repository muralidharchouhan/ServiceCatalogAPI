# ServiceCatalogAPI

This ASP.NET Core Web API mocks ServiceNow-style service catalog item management. It provides endpoints for creating, updating, viewing, and deleting catalog items, stores data in a local JSON file, and includes Swagger for testing. The project is ready for Azure deployment.

## Features
- ServiceNow-style API endpoints
- Local JSON file storage
- Swagger UI for testing
- Azure-ready structure

## Usage
1. Build and run the project.
2. Use Swagger UI to test endpoints.
3. Catalog items are stored in `catalog.json` in the project root.

## Endpoints
- `POST /api/catalogitem/create` - Create a catalog item
- `PUT /api/catalogitem/update/{id}` - Update a catalog item
- `GET /api/catalogitem/view/{id}` - View a catalog item
- `DELETE /api/catalogitem/delete/{id}` - Delete a catalog item

## Deployment
- Publish to Azure App Service using standard ASP.NET Core deployment steps.

## Notes
- This is a mock implementation for development/testing purposes.
