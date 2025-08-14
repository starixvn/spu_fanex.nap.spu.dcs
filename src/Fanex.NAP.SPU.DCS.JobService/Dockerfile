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

# Create non-root user
RUN addgroup -g 1001 -S appgroup && \
    adduser -u 1001 -S appuser -G appgroup

WORKDIR /app

COPY --from=builder /app .

# Change ownership of the app directory to the non-root user
RUN chown -R appuser:appgroup /app && \
    chmod -R 755 /app

ENV TZ="America/La_Paz"

# Switch to non-root user
USER appuser

ENTRYPOINT ["dotnet", "Fanex.NAP.SPU.DCS.JobService.dll"]