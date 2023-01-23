FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY Directory.Build.props .

COPY ["AuditingSubWorker/AuditingSubWorker.csproj", "AuditingSubWorker/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["MessageBus/MessageBus.csproj", "MessageBus/"]
RUN dotnet restore "AuditingSubWorker/AuditingSubWorker.csproj"
COPY . .
WORKDIR "/src/AuditingSubWorker"
RUN dotnet build "AuditingSubWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuditingSubWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuditingSubWorker.dll"]
