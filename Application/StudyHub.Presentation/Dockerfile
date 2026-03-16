FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StudyHub.Presentation/StudyHub.Presentation.csproj", "StudyHub.Presentation/"]
COPY ["StudyHub.Core/StudyHub.Core.csproj", "StudyHub.Core/"]
COPY ["StudyHub.Domain/StudyHub.Domain.csproj", "StudyHub.Domain/"]
COPY ["StudyHub.Infrastructure/StudyHub.Infrastructure.csproj", "StudyHub.Infrastructure/"]
RUN dotnet restore "StudyHub.Presentation/StudyHub.Presentation.csproj"
COPY . .
WORKDIR "/src/StudyHub.Presentation"
RUN dotnet build "./StudyHub.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StudyHub.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudyHub.Presentation.dll"]
