FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /app

COPY . .

RUN dotnet publish sp-api-profanity-filter/sp-api-profanity-filter.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as deployable
RUN apt-get update && apt-get upgrade -y

WORKDIR /app
COPY --from=build-env /app/out .
COPY version .
EXPOSE 5000

FROM deployable
ENTRYPOINT ["dotnet", "sp-api-profanity-filter.dll"]
