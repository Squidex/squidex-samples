#
# Stage 1, Prebuild
#
FROM microsoft/aspnetcore-build:2.0.3-jessie as builder

COPY . .

WORKDIR /

# Publish
RUN dotnet restore \
 && dotnet publish Sample.Profile/Sample.Profile.csproj --output /out/ --configuration Release

#
# Stage 2, Build runtime
#
FROM microsoft/aspnetcore:2.0.3-jessie

# Default AspNetCore directory
WORKDIR /app

# Copy from nuild stage
COPY --from=builder /out/ .

EXPOSE 80

ENTRYPOINT ["dotnet", "Sample.Profile.dll"]