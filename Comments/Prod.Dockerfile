FROM mcr.microsoft.com/dotnet/aspnet:10.0.7-resolute-amd64

WORKDIR /app

# Copy published backend
COPY ./published-app ./

# Copy built frontend files
COPY ./ClientApp/build ./ClientApp/build

# App listens on port 8080
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Comments.dll"]