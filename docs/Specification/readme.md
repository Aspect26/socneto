# SOCNETO specification

For more than a decade already, there has been an enormous growth of social networks and their audiences. As people post about their life and experiences, comment on other people’s posts and discuss all sorts of topics, they generate a tremendous amount of data that are stored in these networks. It is virtually impossible for a user to get a concise overview about any given topic.

This project offers a framework allowing the users to analyze data related to a chosen topic from given social networks.

```
GuideLines
- Neodkazujte se na Wikipedii (ani v diplomkách apod.). Buď dejte odkaz na něco sofistikovaného (specifikaci, knihu, o;dborný článek, web produktu apod.) nebo nic. 
- Odkazy by bylo dobré dělat standardním způsobem ([2, 16, 4] a číslovaný seznam na konci)
```

## High level description

Socneto is an extensible framework allowing user to analyse content across multiple social networks.This project tackles the problem of creating concise overview and designing a multi-purpose platform. It aims to give the user an ability to get a glimpse of prevailing public opinion concerning a given topic in a user-friendly form.

Social networks offer free access to data, although the amount is limited by number of records per minute and age of the post which restrict us from downloading and analyzing large amount of historical data. 

To adapt to those limitations, Socneto offers continuous analysis instead of one times jobs. It continuously downloads data and updated reports 

![endless-pipeline](images/endless-pipeline.png)

The project supports only limited types of data analyses such as topic extraction and sentiment analysis supporting english and czech languages. 

In terms of data acquisition, Socneto supports two main social networks Twitter and Reddit. Both of them support limited free API or unlimited API for users who have paid accounts. 

If user requires additional analysis to be made or any other data to be downloaded, it can be done by extending the framework by users own implementation. 

## Use case 

Generally, a user specifies a topic of interest, required analysis and social network to be used. The system then starts collecting and analyzing data. User can then see summary in form of sentiment chart, significant keywords or post examples with the option to explore and search through them. 

A typical use cases is studying sentiment about a public topic (traffic, medicine etc.) after an important press conference, tracking the opinion evolution about a new product on the market, or comparing stock market values and the general public sentiment peaks of a company of interest.

The framework runs on premises thus a user is responsible for handling security and connecting to storage compliant with GDPR rules. 

## An architectural overview

This project is intended to be an application allowing user to add its own data sources, their expected analysis and define visualization which is reflected in the project architecture. 

![layers](images/socneto-layers.png)

_This picture shows conceptual separation of application responsibilities. The most important part is to develop the data processing platform, then to properly analyse the data and present them to the user. (we are aware that the customer might think otherwise)_

The essential part of this project is a data processing platform responsible for data acquisition, data storage and cooperation among all components to successfully deliver any results to the user. 

In order to interpret acquired data correctly an analysis is performed. There are many possible approaches how to analyse data from the internet, thus analyses has to be extensible by the user to fit his needs. 

Analysed data are then presented to the user in a concise form supported by visualizations and sample data.

Requirements stated above are solved by various cooperating modules. Those modules are connected together forming a pipeline with modules dedicated to data acquisition, analysis, persistance and also to module managing the pipeline's behavior. The components will be described in details in a chapter [Platform](##Platform)

![simplified pipeline](images/socneto-arch-simple.png)
_Diagram shows simplified pipeline processing data_


## Planning

The following section offers and insight into the team composition, members' responsibilities and project milestones.

### Team

|Name|Responsibilities|
|:--|:--|
|Irena Holubová| Supervisor|
|Jan Pavlovský|Machine learning engineer, software engineer – builds the platform with a focus on machine learning integration|
|Petra Doubravová|Machine learning, linguistic specialist – develops the sentiment analysis model |
|Jaroslav Knotek|Software engineer – designs and builds the platform|
|Lukáš Kolek|Data engineer – designs and develops the data storage|
|Július Flimmel|Web engineer – builds the web application and frontend|

### Development process

The development follows agile practices. In the beginning, the team will meet every week to cooperate on an analysis and an application design. 

Once the analysis turns into specification and is defended, the team divides into two group cooperating on separate parts data platform and machine learning. At this point team will meet once in two week or when required. 

Best results are achieved when the team works together. The cooperation will be encouraged by day-long workshop when the whole team meets at one place personally in order to progress.

The process of development is divided into the following approximately equally long milestones.

#### Asynchronous communication PoC

The application relies upon asynchronous communication which should be tested in a production environment which requires to get access to an infrastructure with multiple machines. 

Result should prove that the idea is  plausible. At the beginning, the test will feature only test data acquisition component and a test analyser but as the development advances, they will get replaced with production version and the other types components will get connected as well. 

The result of this phase will be working simplified platform for a data flow implementation. The platform will be capable of coordinating all components and allowing us to create a pipeline in order to implement solid data flow.

This task is responsibility of Jaroslav Knotek and Lukáš Kolek.

At the same time, samples of front-end and analysers will be developed by Július Flimmel and Petra Doubravová respectively.

#### Data flow

As the platform stabilizes, more focus is put to proper data acquisition, storage and querying. When user submits job, all components have to cooperate in order to deliver expected results.

At this point, storage is build to store all data e.g. data from social network and application data needed for a proper job execution. It will be followed by proper implementation of data acquisition component downloading data and feeding them to an analyser. This will be also responsibility of Jaroslav Knotek and Lukáš Kolek.

The sentiment analyser is expected to be the most complex and the most risky part of the whole project. It will require great deal of effort to develop, test and finally integrate it into the framework. This will be supervised by Petra Doubravová and Jan Pavlovský.

At this point, first result will start to emerge. To visualize them a front-end will be developed by Jůlius Flimmel.

#### Polishing

The last phase focuses on extendibility and deployment. The application will be extended by the other data acquisition component connecting to Reddit, and a analyser covering simple topic extraction. 

Once all the component are ready, project will start to finalize with testing sessions, documenting and creating deploying guides helping user to start the project from zero. 

## Supported analyses

**TODO Petra**

## Platform architecture

The framework has on service oriented architecture (acronym SOA, see [wiki](https://en.wikipedia.org/wiki/Service-oriented_architecture)). SOA allows for versatility, separation of concerns and future extension.

Communication between the services is provided by distributed publish/subscribe message broker Kafka (see [Documentation](https://kafka.apache.org/documentation/)). It will allow us to create flexible data flow pipeline since components are not required to know predecessor or successor.

<basic kafka flow picture>


### Storage

**TODO Lukas**

- mention data retention
- what will be stored
  - posts and analysed data
  - app data

### Components

Each service has unique responsibility given by functional requirements (ref ...).


* API gateway - Exposes [REST](https://en.wikipedia.org/wiki/Representational_state_transfer) api allowing users to submit jobs and query results of their jobs.
* Data Acquirer - downloads requested data from a given source
  * This module can be extended using client-implemented adapters 
* Analyser - performs sentiment analysis of a given text
* Database storage - stores analysed data along with source text. Also stores application data.
* Job management service - contains all jobs necessary to successful job execution.

#### Data acquirer

**TODO Jara**

- requires credentials
- what will be implemented 
- what will be integrated

> Pokud už víte, přidala bych konkrétní fakta např. k databázovým systémům (jaké konkrétně, jaká mají případně omezení apod.), ke zdrojům dat (co povolují stáhnout, jak apod.). 

#### Analysers

**TODO Jara**

#### Storage wrappers

**TODO Lukas**

### Communication/Cooperation

As was previously stated, data are exchanged using message broker. The main reason to adopt it was its suitability to event driven systems. The framework fires multiple events to which multiple (at the same time) components can react. In our case, data can be acquired from multiple data sources at the same time and send to multiple analysis modules. This complex can be implemented using standard request/response model but more elegant solution is to use publish/subscriber model.

It offers components to subscribe to a topic, to which producer sends data. Multiple producers can publish data to the same topic and kafka makes sure to deliver them to all subscriber. It also keeps track of which message was delivered and which was not delivered yet, so if any component temporarily disconnects, it can continue at work once the connection is up again. 

<event diagram opposing to http approach >

Another benefit of message broker is that particular services does not aware of a sender of its input data and of receiver of its output. It makes it easy to configure data flow.

**TODO all**: mention complexity and what we will actually implement. Don't be too specifit

<!-- The system is implemented as docker containers (expecting single application in each container). Each container is given an address of a the job management service.

When a container starts it requests(pull) a network configuration from the job management service. The configuration contains input and output topics for a given module e.g. module must be registered before its first use. 

Also, when the container starts, it must register job configuration callback to receive configuration of each submited job.


When user submits job on UI, it gets to a Job Management Service(foreman?). It will parse the job and **distributes**(push) respective configuration to all nodes.

### Configuration

Two types
- node [obsolete]
- job

[obsolete]
Node config contains[**TODO** remove]
- input topic(which the node listens to)
- output topic(into which it produces)


Job configs varies for each component type (if flexible pipeline then for each node)(acquirer, analyser and storage). When the job starts, each component receives tailored configuration(that's why they have to be registered).

*From top of my head*

Acquirer job config
- Networks credentials

Analyser
- dunno 

Storage
- dunno
- 
Rules

- on startup
  - job config 2 node (node is alias for any component) channel is established
    - defined by name 
    - one way
  - node accepts configuration or control message (input output topic) per job subnmit (which also contains input and output topic **allows for flexible pipeline**)
- fixed configuration per node
  - NodeUniqueId (prefix for the topics + metrics identifier)
  - MetricCollectorTopic
- Metrics
  - collects events and failures

What should user know now
 -->

### API 

What is backend and what it offers (conceptually)
**TODO Július**

### Front end 

## Frontend

The primary purpose of our frontend is to provide a user a tool to configure and look at multiple different visualisations of the analyzed data. The application will also allow the user to specify and submit new jobs to the platform and will inform him about their progress. The last functionality allows administrators to manage and configure individual components of the whole platform.

### DartAngular + Material
We chose to make the frontent as a *web application* to develop a cross-platform software which is user friendly and easily available. We chose to use a modern and widely used style guidelines / library *Material Design* to quickly build nice and proffesional looking product. We stick with *DartAngular* because its library provides us with [angular components](https://dart-lang.github.io/angular_components/) which already use Material Design.

### Components
Angular uses a component-based architecture, so each page is composed of multiple components. In this section we provide a description of the views the user can encounter and the components they are made from.

#### Login
As mentioned in the API section, the backend requires the user to be authorized for most requests. Therefore the user needs to login before he starts using the application. After signing in with correct combination of username and password, the user is redirected to his dashboard. The credentials are also stored in localstorage, so the user does not need to insert them on each page reload. 

If a user tries to access content to which he is not authorized (receives HTTP result with status code *401*), he is immediatelly redirected to special page, where he is informed about it. From there, he can try visit different content or sign in again.
![Login page](./images/fe_login.png)

#### Dashboard
The Dashboard displays history of all jobs the user ever submitted including a simple information about them (name, submission date). After selecting a specific job, a component with more details about the job and data visualisations are shown. The list of jobs also contains a button which shows component for submitting a new job.
![Dashboard](./images/fe_dashboard.png)

#### New job
This component aims to easy, user-friendly ability to submit a new job. We use Material input components to provide the user with the best UX. The user can specify *name* of the job, *topic* to be searched for, which registered SOCNETO *components* to use (analyzers and data acquirers) and number of posts to fetch, or unlimited.
![Submit new job](./images/fe_submit.png)

#### Job detail
The job detail component contains list of user specified visualisations of the analyzed data. It also contains a paginated list of all acquired posts and their analyses.

At first, the component contains no visualisations. The user has to specify which data from the analyses he wants to be visualized. This approach gives the user a great degree of freedom instead of being presented by hardwired charts. When creating a new chart, the user only has to select the type of chart (pie chart, line chart, ... TODO: which types we want to support?), and write a JSONPath to the attrribute to be visualised (see Analyser output part). These definitions of charts are then stored in our storage, so the user does not need to create them each time.

TODO: screen maybe (list of visualisations, new visualisation, list of posts)

#### Admin
TODO: write more here (how to access it, how does it work?)
Only users with admin privileges are able to access this component. It serves to make the user able to add, remove or configure data analyzer and data acquirer components.


### Features

- extensibility
  - TODO Analyses
  - TODO Data sources
  - TODO Visualization
  - how does it work
- ELK Metrics
  - what we will track
  - 3rd party solution
  - impacts every component

## Development

This section describes what tools will be used in order to comply with software engineering best practises.

### Deployment

Services are deployed in form of container. For the deployment and following container management [Kubernetes](https://kubernetes.io/) orchestration system is employed. 

### Versioning

Code is versioned using Git repository.
- code reviews
- branching model

### CI/CD and automation

CI/CD is implemented using [TeamCity](https://www.jetbrains.com/teamcity/). It allows for a deployment pipeline definition in which code in the repository get automatically tested, then containerized and deployed. This pipeline is triggered by push to a given branch of the repository.

### Testing

What we want to focus on during testing