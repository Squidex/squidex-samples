#
# Stage 1, Prebuild
#
FROM mcr.microsoft.com/dotnet/sdk:10.0 as builder

WORKDIR /

# Copy csproj first to speed up restore
COPY Sample.Profile.slnx .
COPY Sample.Profile/Sample.Profile.csproj Sample.Profile/

RUN dotnet restore

# Copy all other stuff
COPY . .

# Publish
RUN dotnet publish Sample.Profile/Sample.Profile.csproj -c Release -o /app/publish

#
# Stage 2, Build runtime
#
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Default app directory
WORKDIR /app

# Copy from build stage
COPY --from=builder /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Sample.Profile.dll"]