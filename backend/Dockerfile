FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY LeaveManagement.sln ./
COPY src/LeaveManagement.API/LeaveManagement.API.csproj src/LeaveManagement.API/
COPY src/LeaveManagement.Application/LeaveManagement.Application.csproj src/LeaveManagement.Application/
COPY src/LeaveManagement.Infrastructure/LeaveManagement.Infrastructure.csproj src/LeaveManagement.Infrastructure/
COPY src/LeaveManagement.Domain/LeaveManagement.Domain.csproj src/LeaveManagement.Domain/
RUN dotnet restore LeaveManagement.sln

COPY src ./src
RUN dotnet publish src/LeaveManagement.API/LeaveManagement.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "LeaveManagement.API.dll"]
