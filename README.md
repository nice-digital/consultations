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
  - [In-dev integration](#in-dev-integration)
- [Set up](#set-up)
  - [Gotchas](#gotchas)
- [Tests](#tests)
- [Good to know](#good-to-know)
  - [Environments](#environments)

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

Consultations sits below [Varnish](https://github.com/nhsevidence/varnish) so is under the main niceorg domain. It pulls data from [InDev](https://github.com/nhsevidence/publicationsindev) via an API and stores data in SQL Server.

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
    - [xUnit.net](https://xunit.github.io/) for .NET unit tests
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
- [NICE Design System](https://nhsevidence.github.io/nice-design-system/) for NICE styling
    - [NICE Icons](https://github.com/nhsevidence/nice-icons) for icon webfont

 
## Set up
1. Install [KDiff](http://kdiff3.sourceforge.net/) to be able see diffs from integration tests
2. Clone the project `git clone git@github.com:nhsevidence/consultations.git`
3. Open *Consultations.sln*
4. Press F5 to run the project in debug mode
5. Dependencies will download (npm and NuGet) so be patient on first run
6. The app will run in IIS Express on http://localhost:52679/
7. cd into *consultations\Comments\ClientApp* and run `npm start` if Startup is using `UseProxyToSpaDevelopmentServer`. This runs a react dev server on http://localhost:3000/.
8. Run `npm test` in a separate window to run client side tests in watch mode

### Gotchas
- `spa.UseReactDevelopmentServer` can be slow so try using `spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");` instead within [Startup.cs](Comments/Startup.cs).

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
 	A new column will be created in __EFMigrationHistory to flag that the migration has been run.

## Good to know

### Environments
  
| Environment |  URL  |
| ----------- | :---: |
| Dev         | https://dev.nice.org.uk/consultations/ |
| Test        | https://test.nice.org.uk/consultations/ |

## Supported by

<a href="https://browserstack.com">
<img src="https://image.ibb.co/k7aNvK/browserstack_logo_600x315.png" width="300" alt="BrowserStack Logo">
</a>

We're using BrowserStack's support of open source projects for our day-to-day cross-browser and cross-device testing, and as part of an automated CI environment. <a href="https://www.browserstack.com/open-source">See their support for open source projects</a>.

