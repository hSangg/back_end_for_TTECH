FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ./tech-project-back-end.csproj ./  

# Thực hiện restore các dependencies
RUN dotnet restore "tech-project-back-end.csproj"

# Sao chép toàn bộ mã nguồn vào container
COPY . .

# Chuyển đến thư mục đúng trước khi build
WORKDIR "/src"

# Thực hiện build ứng dụng
RUN dotnet build "tech-project-back-end.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish "tech-project-back-end.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image để chạy ứng dụng
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Sao chép file .env (nếu có)
COPY ./.env .env

# Khởi động ứng dụng
ENTRYPOINT ["dotnet", "tech-project-back-end.dll"]
