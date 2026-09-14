#
# Stage 1, Prebuild
#
FROM mcr.microsoft.com/dotnet/sdk:10.0 as builder

WORKDIR /src

# Copy csproj first to speed up restore
COPY Integration.Localize.slnx .
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
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Default app directory
WORKDIR /app

# Copy from build stage
COPY --from=builder /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Integration.Localize.dll"]