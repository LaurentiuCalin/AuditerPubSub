FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY Directory.Build.props .

COPY ["AuditingPubApi/AuditingPubApi.csproj", "AuditingPubApi/"]
COPY ["MessageBus/MessageBus.csproj", "MessageBus/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "AuditingPubApi/AuditingPubApi.csproj"
COPY . .
WORKDIR "/src/AuditingPubApi"
RUN dotnet build "AuditingPubApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuditingPubApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuditingPubApi.dll"]
