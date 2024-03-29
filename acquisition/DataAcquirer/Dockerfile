FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100 AS build
WORKDIR /src
COPY ["WebApi.Twitter/WebApi.Twitter.csproj", "Api/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "Api/WebApi.Twitter.csproj"
COPY . .
COPY ["WebApi.Twitter/*", "Api/"]
WORKDIR "/src/Api"
RUN dotnet build "WebApi.Twitter.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebApi.Twitter.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApi.Twitter.dll"]
