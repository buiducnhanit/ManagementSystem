FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

#ENV http_proxy="http://proxy.hcm.fpt.vn:80"
#ENV https_proxy="http://proxy.hcm.fpt.vn:80"

WORKDIR /src
COPY ["UserService/WebAPI/WebAPI.csproj", "UserService/WebAPI/"]

COPY ["ManagementSystem.Shared/ManagementSystem.Shared/ManagementSystem.Shared.csproj", "Shared/"]
RUN dotnet restore "UserService/WebAPI/WebAPI.csproj"

COPY . .
WORKDIR "/src/UserService/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

RUN apt-get update && apt-get install -y \
    libkrb5-3 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "WebAPI.dll"]