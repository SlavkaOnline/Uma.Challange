FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["WEBAPI/WEBAPI.csproj", "WEBAPI/"]
COPY ["InfrastructureCache/InfrastructureCache.fsproj", "InfrastructureCache/"]
COPY ["Infrastructure.Contracts/Infrastructure.Contracts.csproj", "Infrastructure.Contracts/"]
RUN dotnet restore "WEBAPI/WEBAPI.csproj"
COPY . .
WORKDIR "/src/WEBAPI"
RUN dotnet build "WEBAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WEBAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WEBAPI.dll"]