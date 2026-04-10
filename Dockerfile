# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY HelpDesk.sln .
COPY src/HelpDesk.Domain/HelpDesk.Domain.csproj src/HelpDesk.Domain/
COPY src/HelpDesk.Application/HelpDesk.Application.csproj src/HelpDesk.Application/
COPY src/HelpDesk.Infrastructure/HelpDesk.Infrastructure.csproj src/HelpDesk.Infrastructure/
COPY src/HelpDesk.API/HelpDesk.API.csproj src/HelpDesk.API/
COPY tests/HelpDesk.Tests/HelpDesk.Tests.csproj tests/HelpDesk.Tests/

# Restore
RUN dotnet restore

# Copy everything and build
COPY . .
RUN dotnet publish src/HelpDesk.API/HelpDesk.API.csproj -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "HelpDesk.API.dll"]
