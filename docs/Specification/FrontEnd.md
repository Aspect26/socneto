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
