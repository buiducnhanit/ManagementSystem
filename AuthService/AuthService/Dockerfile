FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AuthService/AuthService/AuthService.csproj", "AuthService/AuthService/"]

COPY ["ManagementSystem.Shared/ManagementSystem.Shared/ManagementSystem.Shared.csproj", "Shared/"]
RUN dotnet restore "AuthService/AuthService/AuthService.csproj"

COPY . .
WORKDIR "/src/AuthService/AuthService"
RUN dotnet build "AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

RUN apt-get update \
    && apt-get install -y libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "AuthService.dll"]