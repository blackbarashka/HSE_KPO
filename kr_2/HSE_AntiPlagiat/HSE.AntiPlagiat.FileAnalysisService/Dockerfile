FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HSE.AntiPlagiat.FileAnalysisService/HSE.AntiPlagiat.FileAnalysisService.csproj", "HSE.AntiPlagiat.FileAnalysisService/"]
RUN dotnet restore "HSE.AntiPlagiat.FileAnalysisService/HSE.AntiPlagiat.FileAnalysisService.csproj"
COPY . .
WORKDIR "/src/HSE.AntiPlagiat.FileAnalysisService"
RUN dotnet build "HSE.AntiPlagiat.FileAnalysisService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HSE.AntiPlagiat.FileAnalysisService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HSE.AntiPlagiat.FileAnalysisService.dll"]
