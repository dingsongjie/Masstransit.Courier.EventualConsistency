#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Masstransit.Courier.EventualConsistency/Masstransit.Courier.EventualConsistency.csproj", "Masstransit.Courier.EventualConsistency/"]
RUN dotnet restore "Masstransit.Courier.EventualConsistency/Masstransit.Courier.EventualConsistency.csproj"
COPY . .
WORKDIR "/src/Masstransit.Courier.EventualConsistency"
RUN dotnet build "Masstransit.Courier.EventualConsistency.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Masstransit.Courier.EventualConsistency.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Masstransit.Courier.EventualConsistency.dll"]
