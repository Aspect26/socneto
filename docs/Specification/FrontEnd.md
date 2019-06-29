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

