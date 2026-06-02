EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ["./AutoScheduler.API/AutoScheduler.API.csproj", "src/AutoScheduler.API"]
COPY ["./AutoScheduler.Application/AutoScheduler.API.csproj", "src/AutoScheduler.Application"]
COPY ["./AutoScheduler.DataAccess/AutoScheduler.API.csproj", "src/AutoScheduler.DataAccess"]
COPY ["./AutoScheduler.Domain/AutoScheduler.API.csproj", "src/AutoScheduler.Domain"]

RUN dotnet nuget add source /generator/artifacts

RUN dotnet restore "src/AutoScheduler.API/AutoScheduler.API.csproj"

COPY . .

WORKDIR "/src/src/AutoScheduler.API"
RUN dotnet publish "AutoScheduler.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AutoScheduler.API.dll"]