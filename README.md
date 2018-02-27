<!-- NB: run `npx doctoc .` to re-generate the ToC -->

# Comment collection
  
 > Provide a way for NICE stakeholders to comment directly on NICE consultations and documents to provide their viewpoint, both generally and on specific parts; and a way for NICE teams to collate the comments for response which doesn’t require repetitive manual handling.

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
- [Good to know](#good-to-know)
  - [Environments](#environments)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->
</details>
  
## What is it?
TODO

### Background
Consultation is a key part of developing NICE guidance, including NICE quality standards. The consultation processes enable external organisations and individuals to comment on guidance content at specific stages in the development process, and feed into the decision-making process.

### Why are we doing this?
What is our motivation for building this product or service?
- NICE efficiencies
- Increase stakeholder engagement
- Improve quality of responses given by NICE
- Reduce time taken for consultation

### Who are our users?
Who do we think would need to use this product or service?
- NICE teams collecting comments on consultations for response
- Stakeholders who want to provide their views on a NICE document

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
- [.NET Core](https://github.com/dotnet/core) on the server
    - [xUnit.net](https://xunit.github.io/) for .NET unit tests
    - [Shouldly](https://github.com/shouldly/shouldly) for .NET assertions
    - [Moq](https://github.com/moq/moq4) for mocking in .NET
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

### In-dev integration

TODO
  
## Set up
1. Clone the project `git clone git@github.com:nhsevidence/consultations.git`
2. Open *Consultations.sln*
3. Press F5 to run the project in debug mode
4. Dependencies will download (npm and NuGet) so be patient on first run
5. The app will run in IIS Express on http://localhost:52679/
6. cd into *consultations\Comments\ClientApp* and run `npm start` if Startup is using `UseProxyToSpaDevelopmentServer`. This runs a react dev server on http://localhost:3000/.
7. Run `npm test` in a separate window to run client side tests in watch mode

### Gotchas
- `spa.UseReactDevelopmentServer` can be slow so try using `spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");` instead within [Startup.cs](Comments/Startup.cs).
  
## Good to know

### Environments
  
| Environment |  URL  |
| ----------- | :---: |
| Dev         | https://dev-consultations.nice.org.uk |
| Test        | https://test-consultations.nice.org.uk |