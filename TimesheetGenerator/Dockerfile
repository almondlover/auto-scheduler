FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine

WORKDIR /generator

COPY . .

RUN dotnet restore
RUN dotnet build /p:Version=0.1.0 -c Release --no-restore
RUN dotnet pack /p:Version=0.1.0 -c Release --no-restore --no-build -o /generator/artifacts