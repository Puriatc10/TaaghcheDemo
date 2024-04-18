FROM rabbitmq:latest AS builder
RUN rabbitmq-plugins enable rabbitmq_management
FROM rabbitmq:latest
COPY --from=builder /etc/rabbitmq /etc/rabbitmq

EXPOSE 5672 15672

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TaaghcheDemo/TaaghcheDemo.csproj", "./TaaghcheDemo/"]
COPY ["TaaghcheDemo.Infrastructure/TaaghcheDemo.Infrastructure.csproj", "./TaaghcheDemo.Infrastructure/"]
COPY ["MessagingContract/MessagingContract.csproj","./MessagingContract/"]
#COPY ["httpscert.cer", "/usr/local/share/ca-certificates"]
#COPY ["TaaghcheDemo/httpscert.cer", "/app/"]
#RUN update-ca-certificates
RUN dotnet restore "./TaaghcheDemo/TaaghcheDemo.csproj"
COPY . .
WORKDIR "/src/TaaghcheDemo"
RUN dotnet build "TaaghcheDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaaghcheDemo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaaghcheDemo.dll"]