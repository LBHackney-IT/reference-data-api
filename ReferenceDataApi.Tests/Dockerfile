FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

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

CMD dotnet test
