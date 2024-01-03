# Swerva
An HTTP web server implementation with support for HTTPS.

# Disclaimer
I am not an authority in the field of web servers. This project is just for fun and educational purposes, so use at your own discretion.

# Running
The project output type is an executable. You could make it into a class library if you desire so, but for testing it is easier to simply use the command `dotnet run` in your terminal of choice. If you want to have support for HTTPS, you need to provide a certicate. To generate a self signed certificate use following command `dotnet dev-certs https -ep certicatename.pfx -p passwordhere`. After creating a certificate, be sure to update settings.json.