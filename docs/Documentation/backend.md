# Backend

The backend is written in _C#_ using _ASP.Net Core_ library, version 2.2. 

The whole codebase is split into three projects: _Api_, _Domain_ and _Infrastructure_. _Api_ implements backend's JSON REST API, _Domain_ implements business logic and _Infrastructure_ implements infrastructure dependent parts of backend.

## API
API project contains entrypoint of the backend solution in [Program.cs](../../backend/Api/Program.cs), which starts the ASP.Net Core web server. It also contains standard [Startup.cs](../../backend/Api/Startup.cs) which configures server settings (CORS, user authentication) and dependency injection. 

The rest of the project's code is split into controllers and models which are in their separate directories. Controllers define backend's REST API and models contain definitions of objects being sent and received by the API.

### Controllers
The controllers are split according to which entities they operate with. Their role is to define concrete HTTP paths the API exposes, which HTTP methods they allow, and how the bodies of the requests look like. They also verify, whether the user querying the API is authenticated and authorized to do such request. The authentication is implemented via HTTP's _Authorization_ header, using _Basic_ token.

#### Socneto Controller
[SocnetoController](../../backend/Api/Controllers/ChartsController.cs) is a base class for each controller. After handling of each API request it checks whether [ServiceUnavailableException](../../backend/Domain/Services/HttpService.cs) was thrown, and if it was, its sets the response's HTTP status code to 503 (Service unavailable). 

#### Charts controller
[ChartsController](../../backend/Api/Controllers/ChartsController.cs) exposes API for management of charts. It defines requests for getting list of all charts defined for a job, creating a chart for a job and removing a chart from a job. 

#### Components Controller
[ComponentsController](../../backend/Api/Controllers/ComponentsController.cs) exposes API for listing components, which are connected to the SOCNETO platform. It defines one route for getting all currently connected analysers and one for currently connected data acquirers. 

#### Job Controller
[JobController](../../backend/Api/Controllers/JobController.cs) exposes API for retrieving job-specific data. It defines routes for listing all user's jobs, creating and stopping a user's job and getting status of a user's job, whether the job is stopped or running. 

It also defines a route for getting posts acquired within a given job. The request is paginated and returns posts only from a given page and also support multiple filters. To get all the posts, it exposes another route, which returns all the posts in CSV format.

The controller also defines routes for retrieving aggregation and array type analyses for a given job. 

#### Management Controller
[ManagementController](../../backend/Api/Controllers/ManagementController.cs) exposes API which informs whether the backend is running, and which other core components are running in SOCNETO platform.

#### User Controller
[UserController](../../backend/Api/Controllers/UserController.cs) exposes single API route, which is for logging in. It verifies, whether the user which is trying to login exists, and provided correct credentials. 

## Domain
The Domain project implements business logic of the backend. It is split into multiple services. While API project works only with interfaces of these services, their implementation are in this project. Just like the API project, Domain also defines multiple models, which define objects, that are transfered between backend and other SOCNETO components, or between API and Domain projects. 

### Authorization service
[AuthorizationService](../../backend/Domain/Services/AuthorizationService.cs) implements the logic of verifying, whether a given user is able to interact with a given job in any way.

### Http service
[HttpService](../../backend/Domain/Services/HttpService.cs) is a wrapper over C#'s [HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netcore-2.2). It implements utility functions for Get, Post and Put HTTP requests, which automatically check, whether the HTTP response was successful (HTTP status code which indicates success was returned), and deserializes the body of the response to a type specified by generic type. 

It also throws [ServiceUnavailableException](../../backend/Domain/Services/HttpService.cs) if the requested route is unavailable.  

### Storage service
[StorageService](../../backend/Domain/Services/StorageService.cs) implements SOCNETO's storage component REST API. All communication between backend and storage components is handled by this service. 

### Job management service
[JobManagementService](../../backend/Domain/Services/JobManagementService.cs) implements SOCNETO's job management service's component REST API. All communication between backend and JMS is handled by this service. 
It also sets default values for twitter and reddit credentials when submitting a job.

### Charts service
[ChartsService](../../backend/Domain/Services/ChartsService.cs) implements the actual logic behind creating, listing and removing charts for a given job. It queries storage component to retrieve the required data.

### CSV service
[CsvService](../../backend/Domain/Services/CsvService.cs) implements creation of a CSV formatted file from a list of objects of the same serializable type. Serializable type in this context is a type, which uses _Newtonsoft.Json_ library attributes, to mark, how it should be serialized into JSON. This service however uses these attributes to serialize the objects into csv.

### Get analysis service
[GetAnalysisService](../../backend/Domain/Services/GetAnalysisService.cs) implements creation and execution of correct queries to storage, to retrieve analysis of acquired posts with given parameters. A query creation is based on:
  * _Analyser's output format:_ the query must ask for the field of the analysis, which is being populated,
  * _Type of the analysis:_ the analysis can be an aggregation of analyses' field or an array of values of analyses' field, 
  * _Requested result_: the requested result may contain multiple analyses (e.g. line chart can contain multiple lines), in which case multiple queries to storage must be executed, and their results must be correctly merged together.
  
### Job service
[JobService](../../backend/Domain/Services/JobService.cs) implements retrieving jobs statuses and jobs' acquired posts from storage.

### User service
[UserService](../../backend/Domain/Services/UserService.cs) implements authenticating a user from given credentials. It queries storage whether the given user exists. 

### Event tracking
// TODO: write me

## Infrastructure
Infrastructure project implements functionality required by the chosen infrastructure of the platform. It contains implementation of communication via Kafka.

## Build
// TOOD: write me
