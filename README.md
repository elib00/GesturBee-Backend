# GesturBee 🌟🚀🎈

This is a .NET 8 Web API that serves as the backend for the GesturBee frontend. 💻✨🎉

## Prerequisites 🛠️📥🔧

- .NET SDK 8.0+ ([Download Here](https://dotnet.microsoft.com/en-us/download))
- Visual Studio
- SQL Server (if using a database)

## Getting Started 🏁💨🌈

### 1. Clone the Repository

```bash
git clone https://github.com/elib00/GesturBee-Backend.git
cd dotnet-webapi
```

### 2. Install Dependencies 📦⚙️🔄

```bash
dotnet restore
```

### 3. Update Configuration 🔐📄📝

Modify `appsettings.json` or use **User Secrets** to keep sensitive information secure.

```json
{
  "Jwt": {
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "Secret": "your-secret-key"
  },
  "ConnectionStrings": {
    "DefaultConnection": "YourDatabaseConnectionString"
  }
}
```

### 4. Run the Application 🏃‍♂️💻🎯

```bash
dotnet run
```

The API will be available at `https://localhost:7152` or `http://localhost:5228`. 🚀🔥🎶

### 5. Access Swagger (API Documentation) 📖🌐🔍

Navigate to:

```
https://localhost:<port>/swagger
```

## Project Structure 🏗️📂🛠️

```
.
├── Controllers         # API Controllers
├── Models             # Data Models
├── Services           # Business Logic
├── Repository         # Data Access Layer
├── Program.cs         # Entry Point
├── appsettings.json   # Configuration Settings
└── README.md          # Documentation
```

## Authentication & Authorization 🔑🔒✅

This API uses **JWT Authentication** to ensure security. 🎟️🛡️✨

1. Obtain a JWT token from the `/api/auth/login` endpoint.
2. Include the token in the **Authorization** header when making requests:
   ```http
   Authorization: Bearer <your-token>
   ```

## Deployment 🚀🌍📦

To publish the API:

```bash
dotnet publish -c Release -o out
```

Run with:

```bash
dotnet out/YourProject.dll
```

## Contributing 🤝💡🎨

Pull requests are welcome! Please follow the [contribution guidelines](CONTRIBUTING.md). 📝🛠️🌟

## License 📜⚖️✅

This project is licensed under the [MIT License](LICENSE). 🎯📄🎵
