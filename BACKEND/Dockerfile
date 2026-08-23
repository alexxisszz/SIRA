FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SistemaRefuerzo.slnx .
COPY src/SistemaRefuerzo.Domain/*.csproj src/SistemaRefuerzo.Domain/
COPY src/SistemaRefuerzo.Application/*.csproj src/SistemaRefuerzo.Application/
COPY src/SistemaRefuerzo.Infrastructure/*.csproj src/SistemaRefuerzo.Infrastructure/
COPY src/SistemaRefuerzo.Api/*.csproj src/SistemaRefuerzo.Api/
RUN dotnet restore src/SistemaRefuerzo.Api/SistemaRefuerzo.Api.csproj

COPY src/ src/
RUN dotnet publish src/SistemaRefuerzo.Api/SistemaRefuerzo.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "SistemaRefuerzo.Api.dll"]
