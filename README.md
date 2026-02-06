# Contact Manager App

This is a web application for managing manager data. The application allows users to upload lists of managers from CSV files, view them in a user-friendly table, and edit or delete records.

<img width="1920" height="891" alt="image" src="https://github.com/user-attachments/assets/0bdfc702-5ef1-471b-bb8a-424375251519" />


## Features

* **Data Upload:** Import manager data from a CSV file.
* **View:** Display a list of all managers in a table format.
* **Edit:** Ability to modify existing manager details (name, date of birth, phone, salary, etc.).
* **Delete:** Remove records from the database.
* **Validation:** Data integrity checks (file format, required fields, data types).

## Technologies

The project is built on the .NET stack using modern approaches:

* **Framework:** ASP.NET Core MVC (.NET 8)
* **Database:** MS SQL Server
* **ORM:** Entity Framework Core
* **CSV Processing:** `CsvHelper` library
* **Frontend:** HTML, CSS, JavaScript (jQuery), Bootstrap (for styling and responsiveness)

## How to Run

### Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or later) installed.
* SQL Server (LocalDB or a full instance) installed.

### Steps to Run

1. **Clone the repository:**
```bash
git clone <your-repo-link>
cd ContactManagerApp

```


2. **Configure the Database:**
Open the `appsettings.json` file and check the `ConnectionStrings:DefaultConnection` string. By default, it is configured for a local server:
```json
"DefaultConnection": "Server=DESKTOP-95GPI7I;Database=ContactManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"

```


*Change `Server` to your server name if it differs.*

3. **Apply Migrations:**
Create the database and apply migrations using the terminal command:
```bash
dotnet ef database update

```


4. **Run the Application:**
```bash
dotnet run

```


Open your browser and navigate to the address shown in the terminal (usually `http://localhost:5000` or `https://localhost:7001`).

## CSV File Structure

For correct import, the `.csv` file must have the following headers (order does not matter, but column names must match):

```csv
Name,DateOfBirth,IsMarried,Phone,Salary
Ivan Petrenko,1990-05-20,true,+380501234567,15000.00
Maria Koval,1995-11-15,false,+380971112233,20000.50

```

* **Name:** Text field (Required).
* **DateOfBirth:** Date in `YYYY-MM-DD` format.
* **IsMarried:** Boolean value (`true`/`false`).
* **Phone:** Phone number.
* **Salary:** Numeric value (decimal separator is a dot).

## Architecture

* **Controllers:** `ManagersController` handles HTTP requests and interacts with services.
* **Services:**
* `ManagerService`: Contains business logic for CRUD operations.
* `CsvParsingService`: Responsible for parsing and mapping CSV files.


* **Repositories:** `ManagerRepository` abstracts database operations via EF Core.
* **Models:** `Manager` (DB entity) and DTOs for data transfer.
