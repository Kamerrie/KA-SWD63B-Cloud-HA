# Use the official .NET Core 3.1 image as the base image
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

# Set the working directory
WORKDIR /app

# Copy the .csproj file
COPY *.csproj ./

# Restore packages
RUN dotnet restore

# Copy the rest of the application files
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime

# Set the working directory
WORKDIR /app

# Copy the built application
COPY --from=build /app/out ./

# Set the environment variable for Cloud Run
ENV ASPNETCORE_URLS=http://*:${PORT}

# Expose the port
EXPOSE 8080

CMD ["dotnet", "KA_SWD63B_Cloud_HA.dll"]
