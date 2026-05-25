FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
# 🎯 Переключаемся на root, чтобы была возможность устанавливать системные пакеты
USER root
# Обновляем репозитории и ставим базу часовых поясов tzdata
RUN apt-get update && apt-get install -y tzdata && rm -rf /var/lib/apt/lists/*
# Устанавливаем часовой пояс по умолчанию для самого Linux-контейнера
ENV TZ=Europe/Warsaw

# Возвращаем стандартного безопасного не-root пользователя .NET 10
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Weather_Bot/Weather_Bot.csproj", "Weather_Bot/"]
RUN dotnet restore "Weather_Bot/Weather_Bot.csproj"
COPY . .
WORKDIR "/src/Weather_Bot"
RUN dotnet build "./Weather_Bot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Weather_Bot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Weather_Bot.dll"]