# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o .sln e os projetos
COPY *.sln ./
COPY TreeCount.API/*.csproj ./TreeCount.API/
COPY TreeCount.Application/*.csproj ./TreeCount.Application/
COPY TreeCount.Common/*.csproj ./TreeCount.Common/
COPY TreeCount.Domain/*.csproj ./TreeCount.Domain/
COPY TreeCount.Repository/*.csproj ./TreeCount.Repository/

RUN dotnet restore

# Copia todos os arquivos do projeto
COPY . .

# Publica apenas a API (sem os testes)
WORKDIR /src/TreeCount.API
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Variável de ambiente opcional
ENV ASPNETCORE_URLS=http://+:8080

# Expõe a porta usada
EXPOSE 8080

# Define o entrypoint
ENTRYPOINT ["dotnet", "TreeCount.API.dll"]
