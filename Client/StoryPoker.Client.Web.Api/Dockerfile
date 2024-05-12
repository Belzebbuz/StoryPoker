FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8015

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Client/StoryPoker.Client.Web.Api/StoryPoker.Client.Web.Api.csproj", "Client/StoryPoker.Client.Web.Api/"]
COPY ["Server/StoryPoker.Server.Abstractions/StoryPoker.Server.Abstractions.csproj", "Server/StoryPoker.Server.Abstractions/"]
RUN dotnet restore "Client/StoryPoker.Client.Web.Api/StoryPoker.Client.Web.Api.csproj"
COPY . .
WORKDIR "/src/Client/StoryPoker.Client.Web.Api"
RUN dotnet build "StoryPoker.Client.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StoryPoker.Client.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoryPoker.Client.Web.Api.dll"]
