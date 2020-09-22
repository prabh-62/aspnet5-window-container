# https://hub.docker.com/_/microsoft-dotnet-core
FROM  mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY PaymentsService.csproj ./PaymentsService/PaymentsService.csproj
RUN dotnet restore PaymentsService/PaymentsService.csproj

# copy everything else and build app
COPY . ./PaymentsService/
WORKDIR /source/PaymentsService
RUN dotnet publish --no-restore  -c Release --self-contained true -o /app /p:PublishSingleFile=true /p:PublishTrimmed=true /p:DebugType=None /p:DebugSymbols=false

# final stage/image
# Uses the 2004 release; 1909, 1903, and 1809 are other choices
FROM mcr.microsoft.com/windows/nanoserver:2004 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Configure web servers to bind to port 80 when present
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["PaymentsService"]
