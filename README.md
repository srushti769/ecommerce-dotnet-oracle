# E-Commerce Application

Role-based product selling web application using C# .NET MVC and Oracle Database.

## Features

### Admin Module
- Secure login with Role-Based Access Control
- Product Management: Add, Edit, Delete, View
- Order Management: View all orders, update status
- User Management: View registered users
- Dashboard: revenue, order count, product stats

### User Module
- Register and Login
- Browse and Search Products by Category
- Place Orders with quantity selection
- Payment Methods: Cash on Delivery / Online / UPI / Card
- Order History and Status Tracking

## Technology Stack

| Technology              | Usage                        |
|-------------------------|------------------------------|
| C# ASP.NET MVC 5        | Backend Web Framework        |
| Oracle Database 11g+    | Relational Database          |
| Oracle.ManagedDataAccess| Oracle DB Connector (.NET)   |
| Bootstrap 4             | Frontend UI                  |
| Razor Views (.cshtml)   | Server-side Rendering        |
| Session Management      | Auth & Authorization         |

## Project Structure

    ECommerceApp/
    ├── Controllers/
    │   ├── AccountController.cs   (Login, Register, Logout)
    │   ├── AdminController.cs     (CRUD, Orders, Users)
    │   └── UserController.cs      (Browse, Order, History)
    ├── Models/
    │   ├── User.cs
    │   ├── Product.cs
    │   ├── Order.cs
    │   ├── OrderItem.cs
    │   └── Payment.cs
    ├── Views/
    │   ├── Account/               (Login, Register)
    │   ├── Admin/                 (Dashboard, Products, Orders)
    │   └── User/                  (Dashboard, Products, MyOrders)
    ├── Helpers/
    │   └── OracleDbHelper.cs
    ├── Filters/
    │   └── SessionCheckFilter.cs
    └── database/
        └── schema.sql

## Setup

1. Clone this repo
2. Open in Visual Studio 2019/2022
3. Install NuGet: Oracle.ManagedDataAccess
4. Run database/schema.sql in Oracle SQL Developer
5. Update Web.config with your Oracle credentials
6. Press F5 to run

## Default Login

- Admin: admin@shop.com / Admin@123
- User: Register a new account

## Developer

Sam | B.Sc. IT | VNSGU Surat
