#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Identity/03.EndPoints/IdentityTapsiDoc.Identity.EndPoints.V1/IdentityTapsiDoc.Identity.EndPoints.V1.csproj", "Identity/03.EndPoints/IdentityTapsiDoc.Identity.EndPoints.V1/"]
COPY ["Identity/01.Core/IdentityTapsiDoc.Identity.Core.ApplicationService/IdentityTapsiDoc.Identity.Core.ApplicationService.csproj", "Identity/01.Core/IdentityTapsiDoc.Identity.Core.ApplicationService/"]
COPY ["Identity/01.Core/IdentityTapsiDoc.Identity.Core.Domain/IdentityTapsiDoc.Identity.Core.Domain.csproj", "Identity/01.Core/IdentityTapsiDoc.Identity.Core.Domain/"]
COPY ["Identity/02.Infra/IdentityTapsiDoc.Identity.Infra.Data.Command/IdentityTapsiDoc.Identity.Infra.Data.Command.csproj", "Identity/02.Infra/IdentityTapsiDoc.Identity.Infra.Data.Command/"]
COPY ["Identity/02.Infra/IdentityTapsiDoc.Identity.Infra.Data.Query/IdentityTapsiDoc.Identity.Infra.Data.Query.csproj", "Identity/02.Infra/IdentityTapsiDoc.Identity.Infra.Data.Query/"]
RUN dotnet restore "./Identity/03.EndPoints/IdentityTapsiDoc.Identity.EndPoints.V1/IdentityTapsiDoc.Identity.EndPoints.V1.csproj"
COPY . .
WORKDIR "/src/Identity/03.EndPoints/IdentityTapsiDoc.Identity.EndPoints.V1"
RUN dotnet build "./IdentityTapsiDoc.Identity.EndPoints.V1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityTapsiDoc.Identity.EndPoints.V1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityTapsiDoc.Identity.EndPoints.V1.dll"]