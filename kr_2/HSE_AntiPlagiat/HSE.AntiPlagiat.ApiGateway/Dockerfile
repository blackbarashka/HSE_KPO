FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HSE.AntiPlagiat.ApiGateway/HSE.AntiPlagiat.ApiGateway.csproj", "HSE.AntiPlagiat.ApiGateway/"]
RUN dotnet restore "HSE.AntiPlagiat.ApiGateway/HSE.AntiPlagiat.ApiGateway.csproj"
COPY . .
WORKDIR "/src/HSE.AntiPlagiat.ApiGateway"
RUN dotnet build "HSE.AntiPlagiat.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HSE.AntiPlagiat.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HSE.AntiPlagiat.ApiGateway.dll"]
