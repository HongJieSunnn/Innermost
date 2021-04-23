#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Sources/Services/Innermost.MusicHub/Innermost.MusicHub.API/Innermost.MusicHub.API.csproj", "Sources/Services/Innermost.MusicHub/Innermost.MusicHub.API/"]
RUN dotnet restore "Sources/Services/Innermost.MusicHub/Innermost.MusicHub.API/Innermost.MusicHub.API.csproj"
COPY . .
WORKDIR "/src/Sources/Services/Innermost.MusicHub/Innermost.MusicHub.API"
RUN dotnet build "Innermost.MusicHub.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Innermost.MusicHub.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Innermost.MusicHub.API.dll"]