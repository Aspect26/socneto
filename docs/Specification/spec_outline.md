## High level description
Socneto  is an extensible framework allowing user to analyse vast socials networks's content. Analysis and social network data collector can be provided by user. 

Social networks offers free access to data, although the amount is limited by number of records per minute and age of the post which restrict us from downloading and analyzing large amount of historical data. To overcome those limitation, Socneto focuses on continuous analysis. It downloads data as they are published and analyses them immediately offering user up-to-date analysis.

<pic1>

The framework runs on premises which gives the user total control over its behavior. (This environment relies less on a user friendly UI and security. Also the extensibility is done by skilled operator and requires less attention of developers)

### Use case 

Generally, a user specifies a topic for analysis and selects data sources and type of analyses to be used. The system then starts collecting and analyzing data. User can then see summary in form of sentiment chart, significant keywords or post examples with the option to explore and search through them. 

Sample use cases might be studying sentiment about a public topic (traffic, medicine etc.) after an important press conference, tracking the opinion evolution about a new product on the market, or comparing stock market values and the general public sentiment peaks of a company of interest. This project does not serve to any specific user group, it tackles the problem of creating concise overview and designing a multi-purpose platform. It aims to give the user an ability to get a glimpse of prevailing public opinion concerning a given topic in a user-friendly form.

### Architecture - overview 

The framework consists of multiple modules forming a pipeline with modules dedicated to data acquisition, analysis, persistance and also to module managing the pipeline's behavior.

<Lukas's simplified picture>




### Goals

Main goal of this project is to build the platform allowing extensibility and flexibility to users and offer them basic analyses and a complex one proving its usability.

## Architecture
### Platform
The framework has on service oriented architecture (acronym SOA, see [wiki](https://en.wikipedia.org/wiki/Service-oriented_architecture)). SOA allows for versatility, separation of concerns and future extension.

Communication between the services is provided by distributed publish/subscribe message broker Kafka (see [Documentation](https://kafka.apache.org/documentation/)). It will allow us to create flexible data flow pipeline since components are not required to know predecessor or successor.

<basic kafka flow picture>

#### Components
Each service has unique responsibility given by functional requirements (ref ...).


* API gateway - Exposes [REST](https://en.wikipedia.org/wiki/Representational_state_transfer) api allowing users to submit jobs and query results of their jobs.
* Data Acquirer - downloads requested data from a given source
  * This module can be extended using client-implemented adapters 
* Analyser - performs sentiment analysis of a given text
* Database storage - stores analysed data along with source text. Also stores application data.
* Job foreman(předák) - contains all jobs necessary to successful job execution.


#### Communication/Cooperation

As was previously stated, data are exchanged using message broker. The main reason to adopt it was its suitability to event driven systems. The framework fires multiple events to which multiple (at the same time) components can react. In our case, data can be acquired from multiple data sources at the same time and send to multiple analysis modules. This complex can be implemented using standard request/response model but more elegant solution is to use publish/subscriber model.

It offers components to subscribe to a topic, to which producer sends data. Multiple producers can publish data to the same topic and kafka makes sure to deliver them to all subscriber. It also keeps track of which message was delivered and which was not delivered yet, so if any component temporarily disconnects, it can continue at work once the connection is up again. 

<event diagram opposing to http approach >

Another benefit of message broker is that particular services does not aware of a sender of its input data and of receiver of its output. It makes it easy to configure data flow.

## Communication

For a communication from FE is used REST. For internal communication between services is used messaging which brings scalability without excessive effort. One exception in the internal communication will be communication with internal SQL storage (see cap. 6).
### Communication architecture
#### Initialization
The system is implemented as docker containers (expecting single application in each container). Each container is given an address of the job management service (Job-config – see architecture schema).
When a service starts in its container it must register itself into Job-config. Job config accepts new services on known topic and requires from services their unique identifier and a basic description. Services are registered and prepared for a future usage. 
#### Submitted job configuration flow
After startup service should listen on an agreed topic where Job-config sends configuration of services when a new job is submitted by a user. The configuration contains description of the job: credentials for social network, analyzer parameters etc. When the configuration is received by service it should be correctly prepared and starts its task.
#### Data and analysis output flow
Along with the configuration described in previous chapter the configuration message contains also topics for data flow. Every post is sent from Data Acquisition into Analyzers and Database in parallel. Analyzers also works in parallel and their output is sent into database.
### Metrics
Messages in the internal communication are sent without waiting for responses. This brings a problem of control of application status (services health and progress). For this propose there is a component which manages metrices. This component listens for messages with services status on a preconfigured topic and user can query these stored logs. Every service should log its progress and errors with some human readable message.
### Extensibility
Described communication architecture brings huge extensibility. A new service lives in its independent container. It only needs to be registered in Job-config without reboot of the whole platform and it must implement the defined API.
(With a few changes in the implementation it is also possible to create custom pipelines of Analyzers if there is need of sequential processing instead of default parallel.)


## Analysis[Petra & Honza]
## API and FE [Julius]
## Storage

TODO

## Responsibilities and planning [all] 
