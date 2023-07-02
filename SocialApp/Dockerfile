FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SocialApp/SocialApp.csproj", "SocialApp/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Persistance/Persistance.csproj", "Persistance/"]
RUN dotnet restore "SocialApp/SocialApp.csproj"
COPY . .
WORKDIR "/src/SocialApp"
RUN dotnet build "SocialApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SocialApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialApp.dll"]
