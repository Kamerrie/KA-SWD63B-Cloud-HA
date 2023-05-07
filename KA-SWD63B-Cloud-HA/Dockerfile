# Use the official .NET Core runtime image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application files
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app/out .

# Expose the port and start the app
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE 8080
CMD ["dotnet", "KA_SWD63B_Cloud_HA.dll"]
