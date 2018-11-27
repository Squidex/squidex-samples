#
# Stage 1, Prebuild
#
FROM microsoft/dotnet:2.1-sdk as builder

COPY . .

WORKDIR /

# Publish
RUN dotnet restore \
 && dotnet publish Sample.Profile/Sample.Profile.csproj --output /out/ --configuration Release

#
# Stage 2, Build runtime
#
FROM microsoft/dotnet:2.1.0-aspnetcore-runtime-alpine

# Default AspNetCore directory
WORKDIR /app

# Copy from nuild stage
COPY --from=builder /out/ .

EXPOSE 80

ENTRYPOINT ["dotnet", "Sample.Profile.dll"]