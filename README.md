This project contains the code for the **Doch-ost** backend server and web client.

# Development

This project is supported on the Windows OS.

## Prerequisites
### .NET core
- [.NET 8.0.11 SDK](https://versionsof.net/core/8.0/8.0.11/)
### dotnet ef
- [dotnet-ef 8.0.11](https://www.nuget.org/packages/dotnet-ef/8.0.11)
```powershell
dotnet tool install --global dotnet-ef --version 8.0.11
```
### Node.js and npm
The web client requires [Node.js](https://nodejs.org/en/download/package-manager). The project
has been tested with the v22.11.0 (LTS).

### environment variables
For generating base64 strings, you'll need to set the environment variable `DCH_SECRET` to a 
secret value. Otherwise, the value provided in the app settings can be used.

### licenses
The application relies on the [Aspose.Total](https://www.nuget.org/packages/Aspose.Total) 
toolkit and requires a license.

A license has been provided for this project but it will expire less than a month.

## Cloning the repository
After dependencies have been installed you will need to clone a local copy of this repository.

```powershell
git clone git@github.com:gnerkus/doch-ost.git
```

## Running the server
### Running with Jetbrains Rider
You can open the solution file and run with `https`.

### Running from the Command Line
To run the server from the command line you can use the `dotnet run` command. An example is 
shown below:
```powershell
cd doch-ost
dotnet ef database update --project Data
dotnet run --project Dochost.Server
```

## Accessing the server and web client
Running the server also starts the web client's server. The servers can be accessed using the 
following URLs:
- backend server: https://localhost:7119
- API documentation: https://localhost:7119/swagger/index.html
- Web client: https://localhost:57813

## Testing the application
The project does not contain any automated tests; manual tests are conducted via the Web client.

### Create a user account
Dochost requires a user account to upload files. A user account can be created via the 
registration form available at the login page _(https://localhost:57813/login)_. The test 
between the form and the submit button toggles the form between login and registration.

### Upload a file
Files can be uploaded with the _Upload_ button at the top-right once logged in. Multiple files 
can be uploaded at the same time.

The loading indicator at the right of a file's row in the list, shows the status of the file's 
preview.

The icon at the left of a file's row indicates the file's type.

### Preview a file
Once the loading indicator on the file is no longer there, clicking on the file's row displays 
a preview pane on the right side of the list.

### Download a file
The file can be downloaded using the download button at the top right of the preview pane. A 
file can be downloaded even if its preview has not yet been generated.

_Only single file downloads are supported_

### Share a file
The share button at the top of the preview pane, besides the download button, allows the 
generation of a public link to the file. Once generated, the link is provided in the input on 
the left of the share button.

Shared files can be accessed by users without a Dochost user account.

Access to a shared file lasts for 5 minutes. This can be configured via the 
`ExpirationDurationMs` field in the app settings.