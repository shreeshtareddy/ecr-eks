# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY CountryCapital.csproj ./
RUN dotnet restore

# Copy the entire project and build it
COPY . . 
RUN dotnet publish -c Release -o /app/out

# Use the .NET Runtime image to run the app
FROM mcr.microsoft.com/dotnet/runtime:9.0

WORKDIR /app

# Copy the build output from the build image
COPY --from=build /app/out .

# Set the entry point to run the app
ENTRYPOINT ["dotnet", "CountryCapital.dll"]