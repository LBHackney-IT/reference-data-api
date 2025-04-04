FROM mcr.microsoft.com/dotnet/sdk:8.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

WORKDIR /app

# Install and run sonar cloud scanner
RUN apt-get update && apt-get install -y openjdk-17-jre
RUN dotnet tool install --global dotnet-sonarscanner --version 5.6.0
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_reference-data-api" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}" /d:sonar.cs.opencover.reportsPaths=**/coverage.xml

# Copy csproj and nuget config and restore as distinct layers
COPY ./ReferenceDataApi.sln ./
COPY ./ReferenceDataApi/ReferenceDataApi.csproj ./ReferenceDataApi/
COPY ./ReferenceDataApi.Tests/ReferenceDataApi.Tests.csproj ./ReferenceDataApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ReferenceDataApi/ReferenceDataApi.csproj
RUN dotnet restore ./ReferenceDataApi.Tests/ReferenceDataApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out ReferenceDataApi.Tests/ReferenceDataApi.Tests.csproj

CMD dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=coverage.xml && dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
