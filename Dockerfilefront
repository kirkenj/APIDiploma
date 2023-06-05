#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV DiplomaDatabaseConnectionString="server=77.246.96.104;user=diploma_usr;password=Mysqlpwd1;database=Diploma;"
ENV DiplomaLocalMySQLConnectionString="server=77.246.96.104;user=diploma_usr;password=Mysqlpwd1;database=Diploma;"

RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt-get update; apt-get install -y ttf-mscorefonts-installer fontconfig

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebFront/WebFront.csproj", "WebFront/"]
COPY ["Logic/Logic.csproj", "Logic/"]
COPY ["Database/Database.csproj", "Database/"]
RUN dotnet restore "WebFront/WebFront.csproj"
COPY . .
WORKDIR "/src/WebFront"
RUN dotnet build "WebFront.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebFront.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebFront.dll"]