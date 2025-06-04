# CoursesManager

CoursesManager is an ASP.NET Core Web API for managing courses.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Installation

1. Clone the repository:

```
git clone https://github.com/Lxzer0/courses-manager.git
cd CoursesManager
```

2. Restore dependencies:

```
dotnet restore
```

## Configuration

The API requires a JWT secret to sign and validate tokens. Set the `JwtSettings__Secret` environment variable before running the application.

### Linux/macOS

```
export JwtSettings__Secret="your_jwt_secret_here"
```

### Windows PowerShell

```
$env:JwtSettings__Secret = "your_jwt_secret_here"
```

## Running Locally

With the environment variable set, run the application:

```
dotnet run
```

By default, the API will listen on:
- HTTP: `http://localhost:5149`
- HTTPS: `https://localhost:7281`

## Usage

You can send requests against the API endpoints (e.g., `/api/users`, `/api/courses`, `/api/categories`) using your tool of choice (cURL, Postman, etc.).

For quick testing, see the `CoursesManager.http` file in the repository root for sample HTTP requests.

## License

This project is licensed under the [MIT License](LICENSE).
