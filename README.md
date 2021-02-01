# Web API Template with DDD pattern

_Set of definitions and protocols that will be use to develop a Web API following the Domain Driven Design pattern._

## Starting 🚀

_This document will allow you to understand the architecture and components of this project. 

## Pre-requirements 📋

_To execute this project you will need the following packages installed in your computer:_

1. Visual Studio 2019
2. NET Core 3.1

## Arquitecture 

_This project is splitted in diferent layers using the Clean Architecture methodology described here (https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures). Here you can find an explanation of each layer:_

1. WhiteLabel.WebAPI: It contains the API controllers. Don't add business logic here. API credentials, connection string database or any other sensitive information shouldn't be stored in the appsettings.json file, use the secrets.json file for this purpose.
2. WhiteLabel.Domain: It contains the high business logic: Entities, Events, Specifications, etc. This layer has to remain isolated from the others and can't contain any reference to any other project.
3. WhiteLabel.Application: It contains the low business logic: DTOs, Services, Interfaces, etc. This layer has to remain isolated from the others, it will contain only a reference to WhiteLabel.Domain
4. WhiteLabel.Infrastructure.Data: It contains the data classes (Repositories). In this project we will work with a Generic class GenericRepository and we won't need to add a repository class per entity.
5. Infrastructure.MigrationHelper: It contains the nuget package Microsoft.EntityFrameworkCore.Tools necessary to be able to execute the EF CodeFirst commands like Add-Migrations, Update-Database, etc.
6. Infrastructure.DependencyInjection: It contains all the dependecy injections. If you add a new service or repository class, you will need to add the injection here using AutoFac. By default all services created in WhiteLabel.Application that inherits from IBusinessService interface will be injected automatically.
7. WhiteLabel.Infrastructure.Events: It contains the classes to manage the events. _TODO. Define better_

## Startup configuration

_TODO. Explain the code of Startup and Program classes_

## Exceptions managing

_TODO. Explain the error management and the Response and GenericError classes_

## Model data validation

_TODO. Explain how data validation is managed using FluentValidation_

## AutoMapper

_TODO. Explain how mapping classes_

## Events

_TODO. Explain how to work with Events_

## How to add a new entity CRUD?

_TODO. Explain how to add a new CRUD (Entity, Specifications, Events, DTO, Service, Validation, Controller, etc.)_

## Good practices

If you are going to add some code to this project, it's very important that you follow these rules:

1. Apply the SOLID principles: SRP, OCP, LSP, ISP and DIP.
2. Leblanc’s law: Later equals never. If you see a mess, fix it.
3. The Boy Scout rule: “Leave the campground cleaner than you found it”.
4. The Stepdown rule: Each function introduces the next.
5. The Einstein rule: Make it as simple as possible but not simpler.
6. Names mean one thing and only one thing.
7. Single level of abstraction. One method, one thing.
8. Comments are to compensate our failure to express ourself in code.
9. Demeter's law: Do not accept candies from strangers
10. Murphy's law: If something can go wrong, it will. (Be pessimistic)

## Entity Framework Code First Migrations

_To be able to execute the EF commands you will need to follow these steps:_

1. Select project Infrastructure.MigrationHelper as startup project in solution explorer.
2. Open console packet manager (PM) and select Website.Infrastructure.Data as default project.
3. Add-Migration <name_migration>
4. Update-Database

## Testing ⚙️

_TODO. In this section we'll explain how to execute the Unit Tests defined in this project._

## Deployment 📦

_TODO. Pending to add information._

## Authors ✒️

Oscar Rodriguez - oscar.chelo@gmail.com<br />
https://oscarchelo.blogspot.com/<br />
https://www.linkedin.com/in/oscar-rodriguez-lopez-70b2a337<br />

