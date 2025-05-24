FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HSE.AntiPlagiat.Web/HSE.AntiPlagiat.Web.csproj", "HSE.AntiPlagiat.Web/"]
RUN dotnet restore "HSE.AntiPlagiat.Web/HSE.AntiPlagiat.Web.csproj"
COPY . .
WORKDIR "/src/HSE.AntiPlagiat.Web"
RUN dotnet build "HSE.AntiPlagiat.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HSE.AntiPlagiat.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HSE.AntiPlagiat.Web.dll"]
