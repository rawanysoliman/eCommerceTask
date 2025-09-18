# eCommerceApp (Angular + .NET 9)

A simple e-commerce demo with Angular frontend and ASP.NET Core backend. It includes authentication, role-based UI (Admin vs User), product listing, product details, and admin CRUD.

## Prerequisites
- .NET 9 SDK
- Node.js 18+

## Backend (ASP.NET Core)
Location: `EcommerceSolution/Ecommerce.Api`

Run:
1. Open a terminal at the repo root
2. `cd EcommerceSolution/Ecommerce.Api`
3. `dotnet run`

URLs (Development):
- HTTPS: `https://localhost:7010`
- HTTP: `http://localhost:5155`
- API base: `https://localhost:7010/api`

Notes:
- The API serves static files from `wwwroot` (e.g., product images under `/images/products`).
- A default admin user is seeded at startup if no admin exists.

Seeded Admin:
- Username: `admin`
- Email: `admin@local`
- Password: `Admin@123`
- Role: `Admin`

Normal Users:
- Register via the frontend Register page (role defaults to `User`).

## Frontend (Angular)
Location: `frontEnd`

Install & Run:
1. Open another terminal at the repo root
2. `cd frontEnd`
3. `npm install`
4. `npm start`

URL: `http://localhost:4200`

The frontend is configured to call the API at `https://localhost:7010/api`.

## How to log in
- Admin: use the seeded account
  - Username or Email: `admin` or `admin@local`
  - Password: `Admin@123`
- User: register a new account on the Register page, then log in with those credentials

## Roles and permissions
- User: can log in and view Products and Product Details
- Admin: can additionally Create, Edit, and Delete products (admin-only buttons visible in the UI)

## Features
- Angular 18 standalone components, routing with guards
- Bootstrap styling
- RxJS state for auth/products (BehaviorSubject + observables)
- Auth Interceptor attaches bearer token and, on 401, attempts one refresh then retries
- Product images resolved against API origin for correct preview

## Configuration

This project uses an `appsettings.json` file for configuration. ** the real `appsettings.json` is not included in the repository.**

### Setup

1. Create a new file called `appsettings.example.json` in the `EcommerceSolution/Ecommerce.Api` folder.
2. Use the following structure as a template (do **not** include real secrets here):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=USER;Password=PASSWORD;"
  },
  "Jwt": {
    "Secret": "YOUR_SECRET_KEY",
    "ExpirationMinutes": 60
  }
}
## Known notes
- Automated tests (Jest / Cypress) were not included due to time constraints. Manual testing of all features has been done, and the application is fully functional.

## Troubleshooting
- If images don’t load, ensure the backend is running and `wwwroot/images/products` contains the files returned by the API (`imageUrl` is relative to the API root).
- If you get CORS errors, back end allows `http://localhost:4200`. Verify the frontend is served on that origin.
- If your session expires, the interceptor will attempt a refresh and retry once; otherwise you will be logged out.
