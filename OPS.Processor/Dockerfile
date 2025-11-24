FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OPS.Processor/OPS.Processor.csproj", "OPS.Processor/"]
COPY ["OPS.Messages/OPS.Messages.csproj", "OPS.Messages/"]
RUN dotnet restore "OPS.Processor/OPS.Processor.csproj"
COPY . .
WORKDIR "/src/OPS.Processor"
RUN dotnet build "./OPS.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OPS.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OPS.Processor.dll"]
