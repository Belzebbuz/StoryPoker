FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8025

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Server/StoryPoker.Server.Silo/StoryPoker.Server.Silo.csproj", "Server/StoryPoker.Server.Silo/"]
COPY ["Server/StoryPoker.Server.Grains/StoryPoker.Server.Grains.csproj", "Server/StoryPoker.Server.Grains/"]
COPY ["Server/StoryPoker.Server.Abstractions/StoryPoker.Server.Abstractions.csproj", "Server/StoryPoker.Server.Abstractions/"]
RUN dotnet restore "Server/StoryPoker.Server.Silo/StoryPoker.Server.Silo.csproj"
COPY . .
WORKDIR "/src/Server/StoryPoker.Server.Silo"
RUN dotnet build "StoryPoker.Server.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StoryPoker.Server.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoryPoker.Server.Silo.dll"]
