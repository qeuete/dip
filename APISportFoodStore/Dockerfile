# Базовый образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Сборка проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APISportFoodStore.csproj", "./"]
RUN dotnet restore "./APISportFoodStore.csproj"
COPY . .
RUN dotnet build "APISportFoodStore.csproj" -c Release -o /app/build

# Публикация проекта
FROM build AS publish
RUN dotnet publish "APISportFoodStore.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APISportFoodStore.dll"]