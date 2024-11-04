# Temel imaj olarak ASP.NET Framework'ü kullan
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019

# Çalışma dizinini ayarla
WORKDIR /inetpub/wwwroot

# Projeyi yayınlama dizininden yayınlanmış dosyaları kopyala
COPY ./bin/Release/net48/publish/ .

# Uygulamayı çalıştır
ENTRYPOINT ["w3wp.exe"]
