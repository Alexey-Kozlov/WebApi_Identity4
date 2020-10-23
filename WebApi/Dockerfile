FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5007

ENV ASPNETCORE_Kestrel__Endpoints__Http__Url="http://ws-pc-70:5007"
ENV ASPNETCORE_URLS=http://+:5007
ENV ConnectionStrings__TestDb="Data Source=172.20.0.2;Database=Identity;User Id=sa;Password=gjkjcfnbrA1$;"
ENV AuthenticationServerUrl="http://Ws-Pc-70:5001"
ENV AllowedCorsUrls="http://ws-pc-70:5005"

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WebApi.csproj", "./"]


RUN dotnet restore "./WebApi.csproj"

COPY . .

WORKDIR "/src/."

RUN dotnet build "WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApi.dll"]



##See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
#WORKDIR /app
#
#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
#WORKDIR /src
#COPY ["WebApi/WebApi.csproj", "WebApi/"]
#RUN dotnet restore "WebApi/WebApi.csproj"
#COPY . .
#WORKDIR "/src/WebApi"
#RUN dotnet build "WebApi.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "WebApi.dll"]