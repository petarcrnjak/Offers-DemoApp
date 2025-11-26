# Offers-DemoApp — Offer CRUD Proof of Concept

Small proof-of-concept web application that implements an Offer (Ponude) CRUD workflow plus Excel import and server-side pagination. The purpose is to demonstrate architecture, coding practices and validation patterns (client + server), not to be a production product.

## Summary of requirements implemented
- Create, edit and view offers.
- Offer header contains:
  - Offer number (generated when the offer is saved to the database)
  - Offer date (default to today on create/import)
- Offer items presented in a grid with columns:
  - Article (selectable from on-demand lookup)
  - Unit price
  - Quantity
  - Total price (unit price * quantity)
- Inline or dialog-based add / edit / delete of items.
- Offer total shown below items (sum of line totals).
- Client-side and server-side validation implemented (example: Quantity > 0).
- Offers list (browse) with server-side pagination (3 records per page). Clicking the offer number opens the edit page.
- REST API for all server-side CRUD operations.
- Create the corresponding database that the web application will use and populate it with test data.
- Excel (.xlsx) import page:
  - User uploads an Excel file containing a list of offers/items.
  - Server validates each row before saving.
  - For a fully valid file: report successful import.
  - For files with invalid rows: return list of errors with line number + validation messages.
  - Duplicate prevention by offer number.
  - Offer date set to today for imported offers.

## API (examples)
- GET /api/offers?page={n}&pageSize={s}
- GET /api/offers/{id}
- POST /api/offers
- PUT /api/offers/{id}
- DELETE /api/offers/{id}
- POST /api/offers/import  — multipart/form-data, Excel file
- GET /api/products?search={term} — on-demand product lookup

Implemented REST API endpoints
- GET /api/offers
  - Returns list of all offers.
- GET /api/offers/paginated?pageIndex={n}
  - Returns paginated offers (server-side pagination with page size = 3 ).
- GET /api/offers/{id}
  - Get a single offer by id.
- POST /api/offers
  - Create a new offer.
- PUT /api/offers/{id}
  - Update an offer
- DELETE /api/offers/{offerId}/items/{itemId}
  - Delete an item from an offer.
- GET /api/offers/offerItems
  - Returns article/item names used to populate the on-demand select/dropdown.
- POST /api/offers/import
  - Import offers in bulk. Excel file, returns success or per-row validation errors.


## Data & validation
- Minimal model: Offer header + OfferItem (Article reference, UnitPrice, Quantity, LineTotal).
- Quantity validation: required, integer/decimal greater than 0 (client + server).
- Duplicate offer numbers prevented at import/save.

## Technology stack (suggested)
- C# / .NET (6+ / 8)
- MVC frontend (task requirement allowed .NET 6+ MVC)
- ASP.NET Core Web API for server
- EF Core with SQL Server
- Optional: jQuery Select2 or a frontend autocomplete component for on-demand article lookup

## Running (brief)
1. Configure DB connection string (environment variable or project secrets).
2. Restore / build:
   - dotnet restore
   - dotnet build
3. Apply EF Core migrations and seed test data:
   - dotnet ef database update --project <InfrastructureProject> --startup-project <ApiProject>
4. Run the API (and client if not hosted):
   - dotnet run --project <ApiProject>

Replace `<InfrastructureProject>` and `<ApiProject>` with your actual project paths.

 ## Import / test files
- The repository contains:
  - sqlScriptV2.txt — SQL schema + test data script (run in SQL Server to populate DB).
  - offers_and_items.xlsx — sample Excel data used for testing (provided as example).
## Notes
- The implementation focuses strictly on the requested functionality: CRUD, validation, server-side pagination, and Excel import with validation and duplicate detection.
- All client libraries used must be open-source (no commercial UI component suites).
