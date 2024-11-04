# ASP.NET Core Runtime ve SDK'sını temel alan bir imaj
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["websitem.csproj", "./"]
RUN dotnet restore "./websitem.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "websitem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "websitem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "websitem.dll"]
