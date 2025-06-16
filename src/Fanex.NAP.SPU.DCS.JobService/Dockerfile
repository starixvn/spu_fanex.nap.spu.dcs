FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder
WORKDIR /src

COPY ["./src/Fanex.NAP.SPU.DCS", "Fanex.NAP.SPU.DCS/"]
COPY ["./src/Fanex.NAP.SPU.DCS.JobService/Fanex.NAP.SPU.DCS.JobService.csproj", "Fanex.NAP.SPU.DCS.JobService/"]

WORKDIR "/src/Fanex.NAP.SPU.DCS.JobService"

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN dotnet restore -v n

COPY ./src/Fanex.NAP.SPU.DCS.JobService .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine-25h1 AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.NAP.SPU.DCS.JobService.dll"]