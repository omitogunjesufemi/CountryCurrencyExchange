
# 🌍 Country Data API

An **ASP.NET Core Web API** designed to aggregate country data, currency exchange rates, and estimated GDP figures. The data is persisted to a database and exposed through REST endpoints with filtering, sorting, and summarization capabilities.

---

## ✨ Features

* **Data Aggregation**: Fetches and combines information from external APIs (Country Details and Exchange Rates).
* **Database Management**: Stores country records with full CRUD support.
* **Advanced Querying**: Filter by region and currency, and sort dynamically (e.g., `gdp_desc`).
* **Summary Generation**: Produces a real-time summary image located at `cache/summary.png`, including:

  * Total record count
  * Top countries by estimated GDP
  * Last refresh timestamp

---

## 🛠️ Prerequisites

* **.NET 8.0 SDK** (or a compatible version)
* **Database**: MySQL (configurable in `CountryDBContext`)
* **External APIs**: Two services consumed via `HttpClientService`, configured in settings or environment
* **Font resource**:

  * Place a `.ttf` font file (e.g., `Arial.ttf`) inside a `Fonts/` directory
  * Set **Copy to Output Directory** to ensure summary image rendering works

---

## 🚀 Getting Started

### 1. Installation

Clone the repository:

```bash
git clone [Your-Repo-Link]
cd WebAPI
```

Restore dependencies:

```bash
dotnet restore
```

Confirm your database connection string in `appsettings.json` and apply migrations if needed.

### 2. Run the Application

```bash
dotnet run
```

The API will typically run at:

* `http://localhost:<port>`
* `https://localhost:<port>`

---

## 💡 API Endpoints

All routes are prefixed with `/countries`.

| Method | Endpoint   | Description                                                                       |
| ------ | ---------- | --------------------------------------------------------------------------------- |
| POST   | `/refresh` | Refreshes all data: fetches source info, updates DB, and generates summary image. |
| GET    | `/`        | Returns all stored country records. Supports filtering and sorting.               |
| GET    | `/{name}`  | Retrieves a detailed record by country name (case-insensitive).                   |
| DELETE | `/{name}`  | Removes a country record by name.                                                 |
| GET    | `/image`   | Serves the summary image if available.                                            |

**Query Parameters:**

* `region`
* `currency`
* `sort` (e.g., `population_asc`, `gdp_desc`)

---

### ✅ Example Queries

| Query                                     | Result                                                 |
| ----------------------------------------- | ------------------------------------------------------ |
| `GET /countries?region=Asia&currency=USD` | Lists Asian countries using USD                        |
| `GET /countries?sort=gdp_desc`   | Lists all countries sorted by Estimated GDP descending |
| `GET /countries/japan`                    | Detailed JSON record for Japan                         |

---

## ⚙️ Architecture

This project follows a standard layered architecture:

* **Controllers**: `CountryController`
  Handles routing and request responses
* **Services**: `CountryService`
  Implements business logic, data merging, and summary generation trigger
* **Repositories**: `CountryRepository`
  CRUD operations through EF Core (`CountryDBContext`)
* **Utilities**: `SummaryGenerator`
  Handles summary image creation using **ImageSharp**

---

## 🧩 Dependencies

Key NuGet packages:

* `Microsoft.AspNetCore.Mvc`
* `Microsoft.EntityFrameworkCore`
* `System.Linq.Dynamic.Core`
* `Newtonsoft.Json`
* `SixLabors.ImageSharp`
* `SixLabors.ImageSharp.Drawing`
