# Frontend (Angular)

This folder contains the Angular UI for the Device Management System.

## Main Features

- Authentication (register, login, logout)
- Devices list, details, create, update, delete
- Device assignment and unassignment actions
- AI-assisted description suggestion in device forms
- Bonus free-text search UI integration

## Prerequisites

- Node.js LTS
- Angular CLI
- Backend API running locally

## Backend Dependency

The Angular UI expects the backend API to be running locally at:

- `http://localhost:5103/api`

(configured in `src/environments/environment.development.ts`)

Most device management routes require authentication.
Use the register page to create an account, or log in with an existing user.

## Run Locally

From this folder:

```bash
npm install
ng serve
```

Open: `http://localhost:4200`

## Build

```bash
npm run build
```
