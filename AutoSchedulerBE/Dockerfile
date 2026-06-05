FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src

# Build Generator library package
COPY ./TimesheetGenerator ./generator

RUN dotnet restore "./generator"
RUN dotnet build "./generator" /p:Version=1.0.0 -c Release --no-restore
RUN dotnet pack "./generator" /p:Version=1.0.0 -c Release --no-restore --no-build -o /src/generator

COPY ["AutoSchedulerBE/AutoScheduler.API/AutoScheduler.API.csproj", "backend/AutoScheduler.API/"]
COPY ["AutoSchedulerBE/AutoScheduler.Application/AutoScheduler.Application.csproj", "backend/AutoScheduler.Application/"]
COPY ["AutoSchedulerBE/AutoScheduler.DataAccess/AutoScheduler.DataAccess.csproj", "backend/AutoScheduler.DataAccess/"]
COPY ["AutoSchedulerBE/AutoScheduler.Domain/AutoScheduler.Domain.csproj", "backend/AutoScheduler.Domain/"]

RUN dotnet nuget add source "/src/generator" -n TimesheetGenerator

RUN dotnet restore "./backend/AutoScheduler.API/AutoScheduler.API.csproj"

COPY ./AutoSchedulerBE ./backend
COPY ./*.csv .

WORKDIR "/src/backend/AutoScheduler.API"
RUN dotnet publish "AutoScheduler.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS final
EXPOSE 80
EXPOSE 443

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AutoScheduler.API.dll"]