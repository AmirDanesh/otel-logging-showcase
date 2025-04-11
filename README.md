# Otel Logging Showcase

This is a minimal ASP.NET Core Web API project demonstrating how to implement **OpenTelemetry** for distributed tracing, metrics, and structured logging. The project includes custom middleware for logging HTTP requests and responses, and is fully containerized with Docker.

## ✨ Features

- ✅ OpenTelemetry integration with:
  - Logs
  - Metrics
  - Traces
- ✅ Custom middleware for logging request/response bodies
- ✅ Configurable via `appsettings.json`
- ✅ Docker support
- ✅ Clean and extendable structure

## 📁 Project Structure

```
├── Controllers/
│   └── WeatherForecastController.cs
├── Middleware/
│   └── RequestResponseLoggingMiddleware.cs
├── Configuration/
│   ├── OpenTelemetryConfig.cs
│   └── OpenTelemetryLoggingConfig.cs
├── Program.cs
├── appsettings.json
├── Dockerfile
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/) (optional)

### Run the project

#### Using .NET CLI

```bash
dotnet restore
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

#### Using Docker

```bash
docker build -t otel-sample .
docker run -p 5000:80 otel-sample
```

## ⚙️ Configuration

All OpenTelemetry-related configuration can be found and customized in:

- `appsettings.json`
- `Configuration/OpenTelemetryConfig.cs`
- `Configuration/OpenTelemetryLoggingConfig.cs`

## 🧠 How It Works

- Tracing, logging, and metrics are configured via OpenTelemetry SDK.
- Middleware `RequestResponseLoggingMiddleware` captures full HTTP traffic.
- Metrics and traces are automatically collected from built-in .NET instrumentation.

## 📌 Technologies

- ASP.NET Core 8
- OpenTelemetry
- Serilog (optional for structured logging)
- Docker

## 📄 License

This project is open-sourced for learning purposes. You are free to fork and modify it.

---

Feel free to suggest improvements or open issues. Happy tracing! 🚀
