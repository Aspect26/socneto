## High level description [Similar to small spec]
Socneto  is an extensible framework allowing user to analyse vast socials networks's content. Analysis and social network data collector can be provided by user. 

Social networks offers free access to data, although the amount is limited by number of records per minute and age of the post which restrict us from downloading and analyzing large amount of historical data. To overcome those limitation, Socneto focuses on continuous analysis. It downloads data as they are published and analyses them immediately offering user up-to-date analysis.

<pic1>

The framework runs on premises which gives the user total control over its behavior. (This environment relies less on a user friendly UI and security. Also the extensibility is done by skilled operator and requires less attention of developers)

### Use case [Similar to small spec]

Generally, a user specifies a topic for analysis and selects data sources and type of analyses to be used. The system then starts collecting and analyzing data. User can then see summary in form of sentiment chart, significant keywords or post examples with the option to explore and search through them. 

Sample use cases might be studying sentiment about a public topic (traffic, medicine etc.) after an important press conference, tracking the opinion evolution about a new product on the market, or comparing stock market values and the general public sentiment peaks of a company of interest. This project does not serve to any specific user group, it tackles the problem of creating concise overview and designing a multi-purpose platform. It aims to give the user an ability to get a glimpse of prevailing public opinion concerning a given topic in a user-friendly form.

### Goals

Main goal of this project is to build the platform allowing extensibility and flexibility to users and offer them basic analyses and a complex one proving its usability.

## Architecture 
The framework consists of multiple modules forming a pipeline with modules dedicated to data acquisition, analysis, persistance and also to module managing the pipeline's behavior.

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
* Job mangement service - contains all jobs necessary to successful job execution.

![Architecture](https://github.com/jan-pavlovsky/SWProject/blob/dev/docs/Specification/images/arch.PNG)

### Communication

As was previously stated, data are exchanged using message broker. The main reason to adopt it was its suitability to event driven systems. The framework fires multiple events to which multiple (at the same time) components can react. In our case, data can be acquired from multiple data sources at the same time and send to multiple analysis modules. This complex can be implemented using standard request/response model but more elegant solution is to use publish/subscriber model.

It offers components to subscribe to a topic, to which producer sends data. Multiple producers can publish data to the same topic and kafka makes sure to deliver them to all subscriber. It also keeps track of which message was delivered and which was not delivered yet, so if any component temporarily disconnects, it can continue at work once the connection is up again. 

<event diagram opposing to http approach >

Another benefit of message broker is that particular services does not aware of a sender of its input data and of receiver of its output. It makes it easy to configure data flow.

For a communication from FE is used REST. For internal communication between services is used messaging which brings scalability without excessive effort. One exception in the internal communication will be communication with internal SQL storage (see cap. 6).

#### Initialization
The system is implemented as docker containers (expecting single application in each container). Each container is given an address of the job management service (Job-config – see architecture schema).
When a service starts in its container it must register itself into Job-config. Job config accepts new services on known topic and requires from services their unique identifier and a basic description. Services are registered and prepared for a future usage. 
#### Submitted job configuration flow
After startup service should listen on an agreed topic where Job-config sends configuration of services when a new job is submitted by a user. The configuration contains description of the job: credentials for social network, analyzer parameters etc. When the configuration is received by service it should be correctly prepared and starts its task.
#### Data and analysis output flow
Along with the configuration described in previous chapter the configuration message contains also topics for data flow. Every post is sent from Data Acquisition into Analyzers and Database in parallel. Analyzers also works in parallel and their output is sent into database.
#### Metrics
Messages in the internal communication are sent without waiting for responses. This brings a problem of control of application status (services health and progress). For this propose there is a component which manages metrices. This component listens for messages with services status on a preconfigured topic and user can query these stored logs. Every service should log its progress and errors with some human readable message.
### Extensibility
Described communication architecture brings huge extensibility. A new service lives in its independent container. It only needs to be registered in Job-config without reboot of the whole platform and it must implement the defined API.
(With a few changes in the implementation it is also possible to create custom pipelines of Analyzers if there is need of sequential processing instead of default parallel.)

## Analysis

## Implemented problems
* Topic modeling
* Setiment analysis
* Langugage detection
We will implement this analysis for czech and english. It should be easily exensible for other languages.

### Data
Big amount of data will be needed for NLP tasks. We do not assume anotating any data, but this will be one of bottleneck of our model performance. 
There are data sets available for english both social nework posts and other short format data (like IMDb). There exists some data for czech too, but they are smaller.

There is need to prepare some train and test data sets - to gather data, clean them, divide into sets.

### Topic modeling (TM)
This is generally a difficult problem for both languages.  
Possible approaches :
* Word frequency + dictionary - quite easy approach, but it can't handle i.e. pronoun references
* Entity linking - is needed to use some knowledge base
* Entity modeling  - knowledge base needed
* Some probabilistic models (Latent Dirichlet Allocation - LDA)

Combination of different approaches will be used:
frequency, LDA and maybe entity linking, as for czech existing knowledge base is not so large. We will use existing libraries and knowledge bases.

Topics for hierarchiacal structures like posts + their comments will be find for post and related comments together.

### Language detection (LD)
Easier than general language detection problem, because we have just three categories  - czech, english, other

We will use existing model library (probably pycld)

With usage of BERT, language decetion will not be needed for SA.

### Sentiment analysis (SA)
Quite subjective task.
OUtputs:
* polarity
* subjectivity

Using BERT:
https://github.com/google-research/bert/blob/master/predicting_movie_reviews_with_bert_on_tf_hub.ipynb


## API 

Api allows user or frontend application to 
- manage jobs
- manage users
- query results
- set platform configuration

### Job Management Api

This part of API takes care of submitting new jobs and reading intermediary or complete results of finished / in progress jobs

#### `POST /api/job/submit`

Expects a query describing what data should be loaded and analysed

Request:
```json
{
  "query": "string"
  // TODO preferred network along with credentials
}
```

Response:
```json
{
  "jobId": "string"
}
```

`GET /api/job/{jobId}/status`

... specify once the api is stable 
### User management API

API calls for logging into the system, retrieving user's jobs, ...

`GET /api/user/{userId}/jobs`
...

### Configuration API
API for configuring the modules of SOCNETO platform


## Frontend

The primary purpose of our frontend is to provide a user with easily readable visualisations of the acquired and analyzed data. The application will also allow the user to specify and submit new jobs to the platform and will inform him about their progress. The last functionality allows administrators to manage and configure individual components of the whole platform.

### DartAngular + Material
We chose to make the frontent as a *web application* to develop a cross-platform software which is as user friendly as possible (TODO: really???). We chose to use a modern and widely used style guidelines / library *Material Design* to quickly build nice and proffesional looking product. We stick with *DartAngular* because its library provides us with [angular components](https://dart-lang.github.io/angular_components/) which already use Material Design.

### Components
Angular uses a component-based architecture, so in this section we provide description of the most important components.

#### Login
When a user opens the homepage for the first time, the login page is displayed. To use the application, he needs to sign in using a username and password. After inserting correct credentials, they are stored in (TODO: cookies or localstorage), and the user is redirected to his dashboard. Storing the credentials improves the UX because the user does not need to enter the credentials every time he opens the application. If, for any reason and at any time, user receives HTTP result *Forbidden* (HTTP code 403), he is immediatelly redirected to the login page and needs to enter the credentials again.
![Login page](https://github.com/jan-pavlovsky/SWProject/blob/dev/docs/Specification/images/fe_login.png)

#### Dashboard
The Dashboard primarilly displays history of all jobs the user ever submitted including a simple informations about them. Both *finished* and *in progress* jobs are displayed here. After selecting a specific job, more details of the job are shown. This component also contains a button for submiting a new job.
![Dashboard](https://github.com/jan-pavlovsky/SWProject/blob/dev/docs/Specification/images/fe_dashboard.png)


#### New job
This component aims to easy, user-friendly ability to submit a new job. We use Material input components to provide the user with the best UX.
![Submit new job](https://github.com/jan-pavlovsky/SWProject/blob/dev/docs/Specification/images/fe_submit.png)

#### Job detail
The job detail component contains multiple visualisations of the analyzed data of the job. If the analysis is not complete yet, it contains the data, which is already analysed together with the percentage of how much data is processed. The individual visualisations are split into multiple tabs.

The first tab shows a linechart of keyword sentiments of analysed posts. Each line represents one keyword. The y-axis displays the keyword's sentiment, and the x-axis shows date. In addition, the user can specify, which keywords he wants to see in the chart. By default it displays the most used keywords in the job's dataset, but the user can remove or add any other keyword apearing in the dataset.
![Job linechart](https://github.com/jan-pavlovsky/SWProject/blob/dev/docs/Specification/images/fe_job_linechart.png)

The second tab contains a paginated list of all analyzed posts / comments / tweets from the job's dataset. In addition to the original data (post's text), it contains list of extracted keywords, and sentiment of  the post.
![Job posts](https://github.com/jan-pavlovsky/SWProject/blob/dev/docs/Specification/images/fe_job_posts.png)

To not break any privacy policies, we are displaying only publicly available data (plus our analysis). 


#### Admin
Only users with admin privileges are able to access this component. It serves to make the user able to add, remove or configure some components of the SOCNETO platform. TODO: how?

--------------------------------------------

The user can filter data by given time interval.

## Responsibilities and planning [all] 

This page offers detailed description of job division.

### Team

| Name | Responsibilities |
|:--|:--|
|Irena Holubová| Supervisor, decision leader|
|Jan Pavlovský|Machine learning engineer, software engineer – builds the platform with a focus on machine learning integration|
|Petra Doubravová|Machine learning, linguistic specialist – develops the sentiment analysis model |
|Jaroslav Knotek|Software engineer – builds the platform|
|Lukáš Kolek|Data engineer – designs and develops the data storage|
|Julius Flimmel|Web engineer – builds the web application and frontend|

## Road map

TODO specify milestones - don't use MD as a unit since it makes no sense at all.
Each milestones should end with functionality reasonably tested and documented

### Milestone 1 - Bare app

Expected Length 2 months

- satisfies basic requirements
  - user can use it with some tweaks and hacks
  - does not offer any extensibility
- very unstable

### Milestone 2 - Stabilization

Expected length: 3mo

- full functionality covered
  - extensibility
- stable but not production ready

### Milestone 3 -

Expected length: 1mo

- Installation guide
- Documentation finalization
- Presentation preparation
  - Test data

