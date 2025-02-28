FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SacraScriptura.sln", "./"]
COPY ["src/SacraScriptura.API/SacraScriptura.API.csproj", "src/SacraScriptura.API/"]
COPY ["src/SacraScriptura.Application/SacraScriptura.Application.csproj", "src/SacraScriptura.Application/"]
COPY ["src/SacraScriptura.Domain/SacraScriptura.Domain.csproj", "src/SacraScriptura.Domain/"]
COPY ["src/SacraScriptura.Infrastructure/SacraScriptura.Infrastructure.csproj", "src/SacraScriptura.Infrastructure/"]
RUN dotnet restore
COPY . .
WORKDIR "/src"
RUN dotnet build "SacraScriptura.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/SacraScriptura.API/SacraScriptura.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SacraScriptura.API.dll"]
