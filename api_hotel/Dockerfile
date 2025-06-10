# Используем официальный образ .NET для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проекта и восстанавливаем зависимости
COPY ["api_hotel.csproj", "./"]
RUN dotnet restore "api_hotel.csproj"

# Копируем все файлы и собираем приложение
COPY . .
RUN dotnet build "api_hotel.csproj" -c Release -o /app/build

# Публикуем приложение
FROM build AS publish
RUN dotnet publish "api_hotel.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Устанавливаем инструменты для работы с PostgreSQL (опционально)
RUN apt-get update && apt-get install -y postgresql-client

# Открываем порт
EXPOSE 80
ENTRYPOINT ["dotnet", "api_hotel.dll"]