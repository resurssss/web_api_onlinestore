FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore ./server/OnlineStore.API/OnlineStore.API.csproj
RUN dotnet publish ./server/OnlineStore.API/OnlineStore.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OnlineStore.API.dll"]