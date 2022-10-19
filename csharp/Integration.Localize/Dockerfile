#
# Stage 1, Prebuild
#
FROM mcr.microsoft.com/dotnet/sdk:6.0 as builder

WORKDIR /src

# Copy csproj first to speed up restore
COPY Integration.Localize.sln .
COPY Integration.Localize/Integration.Localize.csproj Integration.Localize/
COPY CodeGeneration/CodeGeneration.csproj CodeGeneration/

RUN dotnet restore

# Copy all other stuff
COPY . .

# Publish
RUN dotnet publish Integration.Localize/Integration.Localize.csproj -c Release -o /app/publish

#
# Stage 2, Build runtime
#
FROM mcr.microsoft.com/dotnet/aspnet:6.0.0-bullseye-slim

# Default app directory
WORKDIR /app

# Copy from build stage
COPY --from=builder /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Integration.Localize.dll"]