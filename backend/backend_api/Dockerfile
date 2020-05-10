
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /app

COPY ["backend_api.csproj", "backend_api/"]
WORKDIR /app/backend_api
RUN dotnet restore

COPY ./ ./
WORKDIR "/app/backend_api/"
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS runtime
WORKDIR /app
COPY --from=build /app/backend_api/out ./
ENTRYPOINT [ "dotnet", "backend_api.dll" ]
