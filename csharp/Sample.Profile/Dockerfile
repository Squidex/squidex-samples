#
# Stage 1, Prebuild
#
FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster as builder

WORKDIR /

# Copy csproj first to speed up restore
COPY Sample.Profile.sln .
COPY Sample.Profile/Sample.Profile.csproj Sample.Profile/

RUN dotnet restore

# Copy all other stuff
COPY . .

# Publish
RUN dotnet publish Sample.Profile/Sample.Profile.csproj -c Release -o /app/publish

#
# Stage 2, Build runtime
#
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim

# Default app directory
WORKDIR /app

# Copy from build stage
COPY --from=builder /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Sample.Profile.dll"]