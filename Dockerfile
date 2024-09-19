# Build aþamasý
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LeaveManagementSystemNew.csproj", "./"]
RUN dotnet restore "LeaveManagementSystemNew.csproj"
COPY . .
RUN dotnet build "LeaveManagementSystemNew.csproj" -c Release -o /app/build

# Publish aþamasý
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "LeaveManagementSystemNew.dll"]
