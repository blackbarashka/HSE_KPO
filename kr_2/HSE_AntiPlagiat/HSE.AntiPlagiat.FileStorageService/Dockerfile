FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HSE.AntiPlagiat.FileStorageService/HSE.AntiPlagiat.FileStorageService.csproj", "HSE.AntiPlagiat.FileStorageService/"]
RUN dotnet restore "HSE.AntiPlagiat.FileStorageService/HSE.AntiPlagiat.FileStorageService.csproj"
COPY . .
WORKDIR "/src/HSE.AntiPlagiat.FileStorageService"
RUN dotnet build "HSE.AntiPlagiat.FileStorageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HSE.AntiPlagiat.FileStorageService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HSE.AntiPlagiat.FileStorageService.dll"]
