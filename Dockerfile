FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BibleApi.sln", "./"]
COPY ["src/BibleApi.API/BibleApi.API.csproj", "src/BibleApi.API/"]
COPY ["src/BibleApi.Application/BibleApi.Application.csproj", "src/BibleApi.Application/"]
COPY ["src/BibleApi.Domain/BibleApi.Domain.csproj", "src/BibleApi.Domain/"]
COPY ["src/BibleApi.Infrastructure/BibleApi.Infrastructure.csproj", "src/BibleApi.Infrastructure/"]
RUN dotnet restore
COPY . .
WORKDIR "/src"
RUN dotnet build "BibleApi.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/BibleApi.API/BibleApi.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BibleApi.API.dll"]
