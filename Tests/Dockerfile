FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Tests/Tests.csproj", "Tests/"]
RUN dotnet restore "Tests/Tests.csproj"
COPY . .
WORKDIR "/src/Tests"
RUN dotnet build
RUN dotnet test --results-directory ../../Tests/ --logger "trx;LogFileName=test_results.xml" "Tests.csproj"
