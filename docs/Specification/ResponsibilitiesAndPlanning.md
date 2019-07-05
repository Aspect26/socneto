
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

?? ADD Minář

### Development process

The development follows agile practices. In the beginning, the team will meet every week to cooperate on an analysis and an application design. 

Once the analysis turns into specification and is defended, the team divides into two group cooperating on separate parts data platform and machine learning. At this point team will meet once in two week or when required. 

Best results are achieved when the team works together. The cooperation will be encouraged by day-long workshop when the whole team meets at one place personally in order to progress.

The process of development is divided into the following equally long milestones.

#### Asynchronous communication PoC

The application relies upon asynchronous communication which should to be tested in a production environment which requires to get access to an infrastructure with multiple machines. 

Result should prove that the idea is  plausible. At the beginning, the test will feature only test data acquisition component and a test analyser but as the development advances, they will get replaced with production version and the other types components will get connected as well. 

The result of this phase will be more reliable platform for a data flow implementation. The platform will be capable of coordinating all components and allowing us to create a pipeline in order to implement solid data flow.

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

<!-- #### Future work

*!This phase is not part of the software project goal.* 

The project is designed to be easily migrated to a cloud when needed.  -->

