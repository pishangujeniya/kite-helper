# Getting Angular Build
FROM node:16.16.0-alpine AS angular_build
RUN mkdir /home/node/.npm-global
ENV PATH=/home/node/.npm-global/bin:$PATH
ENV NPM_CONFIG_PREFIX=/home/node/.npm-global
RUN npm install --location=global @angular/cli@14.2.7
WORKDIR /kitehelper
COPY ./src/KiteHelper/ClientApp/ .
RUN npm install
RUN ng build --configuration production --output-path=/kitehelper/dist

# Getting Dotnet project build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet_build
WORKDIR /kitehelper
COPY . .
WORKDIR /kitehelper/src/KiteHelper
RUN dotnet restore "KiteHelper.csproj"
RUN dotnet build "KiteHelper.csproj" -c Release -o /app/build -v q
RUN dotnet publish "KiteHelper.csproj" -c Release -o /app/publish


# Getting Final Container ready
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
# EXPOSE 443

FROM base AS final
WORKDIR /app
COPY --from=dotnet_build /app/publish .
COPY --from=angular_build /kitehelper/dist ./wwwroot/
ENTRYPOINT ["dotnet", "KiteHelper.dll"]