@echo off
echo [INFO] Restarting Kong Gateway...
docker compose down
docker compose up -d

echo.
echo [INFO] Starting ASP.NET Core Web API...
dotnet run --project Kong-Gateway.WebApi/Kong-Gateway.WebApi.csproj --launch-profile https
