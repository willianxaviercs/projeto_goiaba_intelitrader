# Get dotnet sdk
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-focal AS build-env
WORKDIR /app

# Copy CSPROJ and restore any dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the project files and build our release
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build-env /app/out .

COPY wait-for-it.sh /bin
RUN chmod +x /bin/wait-for-it.sh

COPY UserApi.pfx /root/.aspnet/https/

# wait to sql server port to be available before starting the app
CMD ["./../bin/wait-for-it.sh", "postgres-server:5432", "--", "dotnet", "UserApi.dll"]

#ENTRYPOINT [ "dotnet", "UserApi.dll"]