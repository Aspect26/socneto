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

In terms of data acquisition, Socneto supports two main social networks Twitter [^1] and Reddit[2]. Both of them support limited free API or unlimited API for users who have paid accounts. 

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

At this point, first result will start to emerge. To visualize them a front-end will be developed by Július Flimmel.

#### Polishing

The last phase focuses on extendibility and deployment as well as improving precision of supported analysers. The application will be extended by the other data acquisition component connecting to Reddit, and a analyser covering simple topic extraction. 

Once all the component are ready, project will start to finalize with testing sessions, documenting and creating deploying guides helping user to start the project from zero. 

## Supported analyses

**TODO Petra**

## Platform architecture

The framework uses service oriented architecture. Each service runs in a separate docker container. Packing services into docker container makes deployment easier since all required dependencies are present inside the container. 

Containers(except front-end and respective back-end) communicate using a message broker Kafka [^3] which allows for high throughput and multi-producer multi-consumer communication. (For more details, please refer to <<Communication>>).

**TODO link intro to the next section**

### Storage

**TODO Lukas**

- mention data retention
- what will be stored
  - posts and analysed data
  - app data

### Acquiring data

A request for data acquisition contains information about requested data in a form of a query, output channels where to send data and optionally credentials if user does not want to use default one. Then the data acquirer starts downloading data until user explicitly stop it. 

Data acquirers works continuously in order to tackle api limits. For example, twitter standard api limits[^5] allows connected application to lookup-by-query 450 posts or access 900 post by id in 15 minutes long interval. Reddit has less strict limits allowing 600 request per 10 minute interval.

Socneto will support acquiring data from both of those sites with use of 3rd party libraries `LinqToTwitter`[^6] and `Reddit.Net`[^7]. Both of them will make it easier to comply with api limits and tackle respective communication. Both of data acquirers will be written in c#.

### Analysers

The analyzers will process each post they receive from the acquirers. After analyzing a post they will send the result to the storage. Because these analyses will be used by other components (mainly frontend), the analyzers' output will need to be in some standardized form. As this output will be mainly processed by a computer, we decided to use a computer friendly JSON format.

The structure of the JSON will need to be robust and simple enough, so that the user of frontend may easily specify which data he wants to visualize using JSONPath. The structure of the output is following:
```json
{
    "analyzer_name": {
        "analysis_1": { "type": "number", "value": 0.56 },
        "analysis_2": { "type": "string", "value": "rainbow" },
        "analysis_3": { "type": "[string]", "value": ["friends", "games", "movies" ] }
    }
}
```
The whole analysis is packed in one object named after the analyzer. As the analyzer may compute multiple analyses at once, each one will be also represented by one object named after the analysis. The object representing the analysis has a strict format. It contains exactly two attributes:
 * *type*: specifying the type of the result. The supported types will be *number* (including integers and floating point values), *string* and *lists* of these two. Lists of lists will *not* be supported,
 * *value*: the actual result of the analysis

There may be multiple analyzers in our framework, so all their outputs are inserted into one analysis object. We don't use arrays of objects but named objects instead, so that the user may easily specify analyzer and analysis in JSONPath when creating a new chart on frontend.

Here we provide an example of a post's complete analysis. It contains analyses from two analyzers - *keywords* and *topic*. Keywords analyzer returns one analysis called *top-10* where the values are found keywords in the post. The topic analyzer returns one analysis, *accuracy*, specifying how much the post corresponds to the given topic.
```json
{
    "keywords": {
        "top-10": {
            "type": "[string]",
            "value": [
                "Friends", "Games", "Movies", ...
            ]
        }
    },
    "topic": {
        "accuracy": {
            "type": "number",
            "value": 0.86
        }
    },
}
```

#### Storage wrappers

**TODO Lukas**

### Communication/Cooperation

As was previously stated, data are exchanged using message broker `Kafka`. The main reason to adopt it was its suitability to event driven systems. The framework fires multiple events to which multiple (at the same time) components can react. 

In our case, data can be acquired from multiple data sources at the same time and send to multiple analysis modules. This complex can be implemented using standard request/response model but more elegant solution is to use publish/subscriber model.

It offers components to subscribe to a topic, to which producer sends data. Multiple producers can publish data to the same topic and kafka makes sure to deliver them to all subscriber. It also keeps track of which message was delivered and which was not delivered yet, so if any component temporarily disconnects, it can continue at work once the connection is up again. 

Another benefit of message broker is that particular services does not aware of a sender of its input data and of receiver of its output. It makes it easy to configure data flow.

Main pitfall of asynchronous communication a lack of feedback from receiver. More specifically sender is not sure whether anyone is actually listening. In order to tackle this problem, Job Management Service(JMS) was introduced. 

Job management is responsible for proper cooperation of all components. JMS is the component that receives a job, defined by user and transforms it to multiple tailor-made requests for all involved components.

For example, if user defines a task that requires sentiment analysis of data from Twitter, JMS delivers job configuration to the component responsible for twitter data acquisition and connects it to the sentiment analysis input. No other component is aware that this particular job is performed. 

When any container starts, the first thing it does is to register itself and provide information about its type and unique id. This way it gets connected to the system and user can select it (in case of data acquirer or analyser). 

To make sure that components are up an running, some system monitoring was implemented.

### System monitoring

**TODO Lukas**

### API 

The backend API works using HTTP protocol and REST architecture. It works as a (protection / access) layer between frontend and storage modules of SOCNETO framework. It provides access to the authorized users to their jobs, their results and visualisation definitions.

#### Authentication and authorization

To provide protection of the data, almost each API call requires the user to be authenticated and authorized for their usage. These calls expect *Authorization* HTTP header to be filled. As perfect data protection is out of scope of this work, we use only *Basic authorization* for simplicity. After authenticating the user the backend also verifies, whether the user is authorized to read the data he is requesting. If not, a message with HTTP status code *401 (Unauthorized)* is returned. If the user is authoried, the requested data is returned.

For frontend to be able to implement login, the backend contains login call. This call expects username and password in body and returns the user object which corresponds to the given credentials. If there is no such user status code *400 (Bad Request)* is returned.

#### Jobs

The API provides calls to request list of user's jobs, their details and their results. Only authorized users are able to see these data. The user is able to access only those jobs (including their details and results) he submitted. 

The API also contains endpoint for submiting a new job. This endpoint expects correct job definition otherwise returns HTTP status code *400 (Bad Request)*. The correct definition contains title, topic, non-empty list of data acquirer components to be used, non-empty list of data analyzer components to be used and number of posts to fetch (if not unlimited). *JobId* is returned if the job was successfuly created.

#### Visualisation definitions

The user needs to be able to store and load the definitions of his visualisations (see Frontend - Job Detail section). The backend provides API to list already defined visualisations of user's job and to define a new one.

### Front end 

The primary purpose of our frontend is to provide a user a tool to configure and look at multiple different visualisations of the analyzed data. The application will also allow the user to specify and submit new jobs to the platform and will inform him about their progress. The last functionality allows administrators to manage and configure individual components of the whole platform.

#### DartAngular + Material
We chose to make the frontent as a *web application* to develop a cross-platform software which is user friendly and easily available. We chose to use a modern and widely used style guidelines / library *Material Design* to quickly build nice and proffesional looking product. We stick with *DartAngular* because its library provides us with [angular components](https://dart-lang.github.io/angular_components/) which already use Material Design.

#### Components
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


### Extensibility

Socneto framework main feature is an extensibility. A user can implement his own data acquirer or an analyser by creating an application that implements 

- component registration flow in order to connect the component to the system
- input/output API

This application can run in a docker as the rest of the system or it just has to be able to connect to the message broker Kafka. 

## References

[^1]: https://twitter.com
[^2]: https://reddit.com
[^3]: https://kafka.apache.org/
[^5]: https://developer.twitter.com/en/docs/basics/rate-limits
[^6]: https://github.com/JoeMayo/LinqToTwitter
[^7]: https://github.com/sirkris/Reddit.NET