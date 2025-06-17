<!-- NB: run `npx doctoc .` to re-generate the ToC -->

# Comment collection

> Our goal is to provide a simple and consistent way of contributing to and collecting stakeholder comments from NICE consultations

<details>
<summary><strong>Table of contents</strong></summary>
<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

- [What is it?](#what-is-it)
  - [Background](#background)
  - [Why are we doing this?](#why-are-we-doing-this)
  - [Who are our users?](#who-are-our-users)
  - [What outcome will users get from this service?](#what-outcome-will-users-get-from-this-service)
  - [What outcome are we looking for?](#what-outcome-are-we-looking-for)
  - [What are our key metrics?](#what-are-our-key-metrics)
- [Stack](#stack)
  - [Architecture](#architecture)
  - [Technical stack](#technical-stack)
- [Set up](#set-up)
  - [Other README files](#other-readme-files)
  - [Secrets](#secrets)
  - [Redis server](#redis-server)
  - [Gotchas](#gotchas)
- [Tests](#tests)
- [Entity Framework Migrations](#entity-framework-migrations)
- [Good to know](#good-to-know)
  - [Further info](#further-info)
  - [Environments](#environments)
  - [Supported by](#supported-by)
  - [Regenerate the Table of Contents](#regenerate-the-table-of-contents)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->
</details>
  
## What is it?
This service provides a way for NICE stakeholders to comment directly on NICE consultations and documents to provide their viewpoint; and a way for NICE teams to request comments and process them for response which doesn’t require repetitive manual handling.

### Background

Consultation is a key part of developing NICE guidance, including NICE quality standards. The consultation processes enable external organisations and individuals to comment on guidance content at specific stages in the development process, and feed into the decision-making process.

### Why are we doing this?

What is our motivation for building this product or service?

- NICE efficiencies
- Increase stakeholder engagement
- Improve quality of responses given by NICE

### Who are our users?

- Internal teams at NICE who consult on the guidance they are producing by collecting comments from external stakeholders
- Our external stakeholders are the people who the guidance and standards that NICE produces affect and who want to provide their views on the development of these, so that their needs and priorities are reflected in the final guidance

### What outcome will users get from this service?

What problem will it solve for people?

- Less manual handling when collating comments
- Less variation for stakeholders between different types of consultation - Secondary
- Able to comment against specific parts of a document or more generically
- Easier for NICE to tell which part of the document is being commented on

### What outcome are we looking for?

What problem will it solve for our organisation?

- Efficiency savings for NICE by reducing manual handling during collation

### What are our key metrics?

What do we need to measure against these outcomes?

- Time taken to collate results
- End to end time taken for consultation process
- Number of comments received
- Number of distinct organisations commenting
- Improved quality of responses / lower error rate

## Stack

### Architecture

Consultations sits below [Varnish](https://github.com/nice-digital/varnish) so is under the main niceorg domain. It pulls data from [InDev](https://github.com/nice-digital/publicationsindev) via an API and stores data in SQL Server.

<!-- See http://asciiflow.com/ -->

<pre>
                  +---------+                                          
        +---------- Varnish -------------+                             
        |         +----|----+            |                             
        |              |                 |                             
        |              |                 |                             
+-------v------+  +----v----+    +-------v-------+    +---------------+
| Guidance Web |  | Orchard |    | Consultations -----> SQL Server DB |
+--------------+  +---------+    +-------^-------+    +---------------+
                                         |                             
                                         |                             
                                         |                             
                                     +---|---+                         
                                     | InDev |                         
                                     +-------+                         
</pre>

### Technical stack

- [Varnish](https://varnish-cache.org/) for infrastructure-level routing
- [.NET Core 2](https://github.com/dotnet/core) on the server
  - [xUnit.net](https://xunit.net/) for .NET unit tests
  - [Moq](https://github.com/moq/moq4) for mocking in .NET
  - [Shouldly](https://github.com/shouldly/shouldly) for .NET assertions
  - [KDiff](http://kdiff3.sourceforge.net/) for diffing approvals tests
- [SQL Server](https://www.microsoft.com/en-gb/sql-server/sql-server-2017) as our database
  - [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) as an ORM
  - [EF Core In-Memory Database Provider](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/) for integration tests
- [React](https://reactjs.org/) for the UI library
  - [Create React App](https://github.com/facebook/create-react-app) for configless React
  - [Jest](https://facebook.github.io/jest/) for JavaScript tests
  - [ASP.NET Core JavaScript Services](https://github.com/aspnet/JavaScriptServices) for rendering JavaScript server side in .NET
  - [ESLint](https://eslint.org/) for JavaScript linting
- [SASS](https://sass-lang.com/) as a CSS pre-processor
- [Modernizr](https://modernizr.com/) for feature detection
- [WebdriverIO](http://webdriver.io/) for automated functional testing
- [NICE Design System](https://nice-digital.github.io/nice-design-system/) for NICE styling
  - [NICE Icons](https://github.com/nice-digital/nice-icons) for icon webfont

## Set up

1. Install [KDiff](http://kdiff3.sourceforge.net/) to be able see diffs from integration tests
2. Install [SQL Server](https://www.microsoft.com/sql-server) and [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)
3. Create a blank database called "Consultations" using SSMS. Set account running visual studio as db_owner (Your domain username or SUDO account if running as administrator)
4. Optionally restore Consultations database from a backup. (if left blank, EF Migrations will run and create everything)
5. Clone the project `git clone https://github.com/nice-digital/consultations.git`
6. Open _Consultations.sln_
7. Create user secrets
   - Right click on project
   - Select 'manage user secrets'
   - Copy and paste the default user secrets from the Secrets section below
8. Replace the {DatabaseServer} and {DatabaseName} in the connection string with the details from the database created in step 3. SQLServer Express often creates an instance, so your local server name might be in the form of {LaptopName}//SQLEXPRESS
9. Ask devops for read access to our current deployment pipeline and copy all the test environment values for the following secrets file sections
	- Logging
	- Feeds
	- WebAppConfiguration
	- Encryption
	- ConsultationList
10. Set WebAppConfiguration PostLogoutRedirectUri to "http://niceorg:81"
11. Set WebAppConfiguration RedirectUri to "http://niceorg:81/signin-auth0"
12. Press F5 to run the project in debug mode
13. Dependencies will download (npm and NuGet) so be patient on first run
14. The app will run in IIS Express on http://localhost:44306/
15. Optionally (but recommended), install [Volta](https://volta.sh/) to ensure the version of node on you machine does not clash with the node version needed for Consultations
16. Open up a powershell terminal
	- cd into _consultations\Comments\ClientApp_
	- run 'npm ci'
	- run `npm start` if Startup is using `UseProxyToSpaDevelopmentServer`. This runs a react dev server on http://localhost:3000/.
17. Optionally, Run `npm test` in a separate window to run client side tests in watch mode
18. You may need to add a line to your hosts file (C:\Windows\System32\drivers\etc\hosts) pointing "niceorg" at 127.0.0.1
19. You may need to create a Self Signed Certificate for "niceorg" on your machine and bind it to port 44306 to stop browser warnings, more detailed instructions below
20. If you don't have it already, you will need to go into Identity Management for the environment you are working with e.g. https://test-identityadmin.nice.org.uk/ and give youself Administrator access to Consultations
21. Install Redis locally on your machine. Instructions below

## Integration With Indev

Consultations requires access to feeds from Indev to function.

### Test Indev

If you have followed the instructions from the Setup section above then consultations should be integrated with test.

The data in Indev test can be patchy. Creating a new consultation is recommended. Instructions to create consultations are listed later in this readme.

### Local Indev

Indev can be integrated into consultations locally so the whole API chain can be debugged.

The following chain of projects will need to be set up and running.

<pre>
┌─────────────────┐    ┌───────────────┐    ┌────────────────┐
│                 │    │               │    │  Guidance      │
│  Consultations  ◄────┼     Indev     ◄────┼  Conversion    │
│                 │    │               │    │  Service       │
└─────────────────┘    └───────────────┘    └────────────────┘
</pre>
	
#### Consultations Project

1. In the user secrets file
	- Change Feeds > IndevBasePath to "https://local-indev.nice.org.uk" in the secrets file

#### Indev Project

1. Clone the project `git clone https://github.com/nice-digital/indev.git`
2. Set up Indev as per its README.md file
3. Make further changes to the web.config
	- Change appSettings > IndevApiIdentifier to "https://test-indev.nice.org.uk"
	- Anywhere http://niceorg appears, change it to https://niceorg:44306
	
#### Guidance Conversion Service Project

1. Clone the project `git clone https://github.com/nice-digital/guidance-conversion-service.git`
2. Guidance Conversion Service should run straight out of git. Indev points to this local service by default.
3. If you have any trouble consult the README.md

### Creating a consultation in Indev

1. Create a new project
2. Go to Timeline and click Consultation
3. Set the start date and end date, ensure that today's date is between the two dates
4. Set the consultation number as 1
5. Hit save
6. Go to Upload and drag in any templated guidance document.
7. Set the type as "Draft Consultation Document (online commenting)"
8. Hit Upload and wait for the message to say "Converted!" then hit Close
9. Schedule the guidance for Now and hit Save
10. go to https://test-indev.nice.org.uk/golive and hit Go Live

If you go to the list page in consultations and search for this consultation it should be there. Clicking on the consultation should take you to the guidance page with all the links to add comments etc...

### Creating Self Signed Certificate for niceorg

Below is a powershell script which will create and bind an SSL certificate for niceorg. This needs to be run as administrator.

Once the script has run, you will need to import the certificate into your Trusted Root Certificates.
1. Log into your sudo admin account
2. type "manage computer certificates" into the search bar, go into "Manage computer certificates", it will ask for admin password again.
3. Open up Personal > Certificates
4. Find the certificate for "niceorg" and export it to a folder locally
5. Open up "Trusted Root Certification Authorities > Certificates"
6. Delete and existing certificates for "niceorg"
6. Right click "Certificates" and go to "All Tasks > Import"
7. Follow the instructions to import the certificate Exported in step 4

If you return to your normal user account and re-start your project, you should not get any more security errors in your browser

```
$cert = New-SelfSignedCertificate `
    -DnsName "niceorg" `
    -CertStoreLocation "cert:\LocalMachine\My" `
    -KeyExportPolicy Exportable `
    -FriendlyName "niceorg" `
    -NotAfter (Get-Date).AddYears(5)

$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root","LocalMachine")
$store.Open("ReadWrite")
$store.Add($cert)
$store.Close()

$guid = [guid]::NewGuid().ToString()

netsh http delete sslcert ipport=0.0.0.0:44306

netsh http add sslcert ipport=0.0.0.0:44306 `
    certhash=$($cert.Thumbprint) `
    appid="{$guid}"
```

### Other README files

-There is another README file in _consultations\Comments\ClientApp_ which goes into more detail if `npm start` does not work immediately.

### Secrets

- First try to copy the secrets file from another developer who has had comment collection working before.
- If you cannot find another developer with a secrets file, you can copy the format below.

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source={DatabaseServer};Initial Catalog={DatabaseName};Persist Security Info=True;Trusted_Connection=True;"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogFilePath": "Serilog-{Date}.json",
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    },
    "UseRabbit": false,
    "UseFile": true
  },
  "AppSettings": {
    "Environment": {
      "Name": "local",
      "SecureSite": "false"
    }
  },
  "Feeds": {
    "ApiKey": "<indev API key>",
    "IndevBasePath": "<indev base path>",
    "IndevPublishedChapterFeedPath": "<feed path>",
    "IndevDraftPreviewChapterFeedPath": "<feed path>",
    "IndevPublishedDetailFeedPath": "<feed path>",
    "IndevDraftPreviewDetailFeedPath": "<feed path>",
    "IndevPublishedPreviewDetailFeedPath": "<feed path>",
    "IndevListFeedPath": "<feed path>",
    "CacheDurationSeconds": 60,
    "IndevIDAMConfig": {
      "Domain": "<InDev IDAM Domain>",
      "ClientId": "<Auth0 Client Id>",
      "ClientSecret": "<Auth0 Client Secret>",
      "APIIdentifier": "<Auth0 API Identifier>"
    }
  },
  "WebAppConfiguration": {
    "ApiIdentifier": "<Auth0 API Identifier for comment collection>",
    "ClientId": "<Auth0 Client Id for comment collection>",
    "ClientSecret": "<Auth0 Client secret for comment collection>",
    "AuthorisationServiceUri": "<Auth0 Authorisation service URI for comment collection>",
    "Domain": "<Auth0 Domain comment collection>",
    "PostLogoutRedirectUri": "<Auth0 Post Logout Redirect Uri>",
    "RedirectUri": "<Auth0 Redirect Uri>",
    "GoogleTrackingId": "<google tracking id>",
    "RedisServiceConfiguration": {
      "ConnectionString": "<redis server URL (127.0.0.1:port if running locally)>",
      "Enabled": true
    }
  },
  "Encryption": {
    "Key": "<Encryption key for encrypting comment text>",
    "IV": "<Initialisation Vector for encrypting comment text>"
  },
  "ConsultationList": {
    "DownloadRoles": {
      "AdminRoles": [ "<List of Admin roles>", "<List of Admin roles>" ],
      "TeamRoles": [ "<List of team roles>", "<List of team roles>", "<List of team roles>" ]
    }
  }
}

```

### Redis server

This application uses a data store called Redis to capture and store Tokens from Auth0. You will need to run a local version of Redis using Chocolatey, A docker/podman container or via WSL at a command prompt. Go to [https://redis.io/docs/getting-started/](https://redis.io/docs/getting-started/) to get started, the instructions are well written.

### Gotchas

- `spa.UseReactDevelopmentServer` can be slow so try using `spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");` instead within [Startup.cs](Comments/Startup.cs).
- Need to ensure that nothing else is running on port 80, otherwise you will encounter a socket exception error when running in debug.
- Exception: OpenIdConnectAuthenticationHandler: message.State is null or empty. -- caused if login is attempted without redis, clear your cookies and login again.
- It might take a few F5's, visual studio restarts and cookie clears to get all the various services/applications to start co-operating
- Make sure you sign into the main consultation window which pops up when you run the project
- Error message saying "Something must have gone slightly wrong!" - Have a look at the logs in Auth0 tenent. This error is coming from the IDAM signin. Check that the ClientId and ClientSecret are correct for the API Identifier on the tenent you are using.

## Tests

The project uses serveral layers of tests:

- C# low-level unit tests, run via xUnit, asserted with Shouldly. See [Comments.Test/UnitTests](Comments.Test/UnitTests).
- C# 'integration' tests (approval based of server rendered HTML), run via xUnit, asserted with Shouldly, diffed with Kdiff. See [Comments.Test/IntegrationTests](Comments.Test/IntegrationTests).
- JavaScript unit tests via jest. See [Comments/ClientApp/src](Comments/ClientApp/src).
- Functional high-level tests, via webdriver.io.

## Entity Framework Migrations

We use Code first Entity Framework migrations to update the consultations database

To update the database

- add a new property to the relevent class in Consultations > Comments > Models > EF
- in visual studio go to Tools > NuGet Package Manager > Package Manager Console
- in the package manager console window run the command Add-Migration [give your migration a useful name] eg Add-Migration AddCommentCreationDate  
  This will create a new migrations script in Consultations > Comments > Migrations
- when the comment collection is next hit the changes in the migration script will be applied to SQL.  
  A new column will be created in \_\_EFMigrationHistory to flag that the migration has been run.

## Good to know

### Further info

See the [Consultations Sharepoint site](https://niceuk.sharepoint.com/sites/External_Consultations)

### Environments

| Environment | URL                                      |
| ----------- | ---------------------------------------- |
| local       | https://local.nice.org.uk/consultations/ |
| Alpha       | https://alpha.nice.org.uk/consultations/ |
| Live        | https://www.nice.org.uk/consultations/   |

### Supported by

<img src="https://cdn.worldvectorlogo.com/logos/browserstack.svg" align="left" width="80" style= "padding-right: 20px" alt="BrowserStack Logo">
We're using <a href="https://browserstack.com">BrowserStack's</a> support of open source projects for our day-to-day cross-browser and cross-device testing, and as part of an automated CI environment. <a href="https://www.browserstack.com/open-source">See their support for open source projects</a>.

### Regenerate the Table of Contents

For further information about the recommended ReadMe structure plus a 'how to' for regenerating the table of contents, follow the instructions in [DIT Engineering - ReadMes](https://niceuk.sharepoint.com/sites/DIT_Engineering/SitePages/Readmes.aspx)
