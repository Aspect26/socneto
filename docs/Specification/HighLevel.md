```
GuidesLines
- Neodkazujte se na Wikipedii (ani v diplomkách apod.). Buď dejte odkaz na něco sofistikovaného (specifikaci, knihu, o;dborný článek, web produktu apod.) nebo nic. 
- Odkazy by bylo dobré dělat standardním způsobem ([2, 16, 4] a číslovaný seznam na konci)
```

## High level description

Socneto is an extensible framework allowing user to analyse content across multiple social networks. Analysis and social network data collector can be provided by user. 

Social networks offers free access to data, although the amount is limited by number of records per minute and age of the post which restrict us from downloading and analyzing large amount of historical data. 

To overcome those limitation, Socneto focuses on continuous analysis e.g. it watches for any new posts or comments to the followed topic. It downloads the data, analyses them, stores the results and updates respective statistics. User can submit several jobs simultaneously.

![endless-pipeline](images/endless-pipeline.png)

The project support only basic form of data analysis such as topic extraction and a complex one - sentiment analysis supporting english and czech languages. In terms of data acquisition, there are two supported social networks Twitter and Reddit. Both of them supports open API with limited access to the data. If user requires additional analysis to be made or any other data to be downloaded, it can be done by extending the framework by own implementation. 

This project does not serve to any specific user group, it tackles the problem of creating concise overview and designing a multi-purpose platform. It aims to give the user an ability to get a glimpse of prevailing public opinion concerning a given topic in a user-friendly form.

## Use case 

Generally, a user specifies a topic for analysis and selects data sources and type of analyses to be used. The system then starts collecting and analyzing data. User can then see summary in form of sentiment chart, significant keywords or post examples with the option to explore and search through them. 

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
|Irena Holubová| Supervisor, decision leader|
|Jan Pavlovský|Machine learning engineer, software engineer – builds the platform with a focus on machine learning integration|
|Petra Doubravová|Machine learning, linguistic specialist – develops the sentiment analysis model |
|Jaroslav Knotek|Software engineer – builds the platform|
|Lukáš Kolek|Data engineer – designs and develops the data storage|
|Julius Flimmel|Web engineer – builds the web application and frontend|

### Development process

The development follows agile practices. In the beginning, the team will meet every week to cooperate on an analysis and an application design. 

Once the analysis turns into specification and is defended, the team divides into two group cooperating on separate parts data platform and machine learning. At this point team will meet once in two week or when required. 

Best results are achieved when the team works together. The cooperation will be encouraged by day-long workshop when the whole team meets at one place personally in order to progress.

The process of development is divided into the following approximately equally long milestones.

#### Asynchronous communication PoC

The application relies upon asynchronous communication which should to be tested in a production environment which requires to get access to an infrastructure with multiple machines. 

Result should prove that the idea is  plausible. At the beginning, the test will feature only test data acquisition component and a test analyser but as the development advances, they will get replaced with production version and the other types components will get connected as well. 

The result of this phase will be working simplified platform for a data flow implementation. The platform will be capable of coordinating all components and allowing us to create a pipeline in order to implement solid data flow.

This task is responsibility of Jaroslav Knotek and Lukáš Kolek.

At the same time, samples of front-end and analysers will be developed by Jůlius Flimmel and Petra Doubravová respectively.

#### Data flow

As the platform stabilizes, more focus is put to proper data acquisition, storage and querying. When user submits job, all components has to cooperate in order to deliver expected results.

At this point, storage is build to store all data e.g. data from social network and application data needed for a proper job execution. It will be followed by proper implementation of data acquisition component downloading data and feeding them to an analyser. This will be also responsibility of Jaroslav Knotek and Lukáš Kolek.

The sentiment analyser is expected to be the most complex and the most risky part of the whole project. It will require great deal of effort to develop, test and finally integrate it into the framework. This will be supervised by Petra Doubravová and Jan Pavlovský.

At this point, first result will start to emerge. To visualize them a front-end will be developed by Jůlius Flimmel.

#### Polishing

The last phase focuses on extendibility and deployment. The application will be extended by the other data acquisition component connecting to Reddit, and a analyser covering simple topic extraction. 

Once all the component are ready, project will start to finalize with testing sessions, documenting and creating deploying guides helping user to start the project from zero. 

