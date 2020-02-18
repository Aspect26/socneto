# Frontend

The frontend is primarily written in [AngularDart](https://angulardart.dev/) library with minority of the code being written in pure JavaScript. The AngularDart part also uses [angular_components](https://dart-lang.github.io/angular_components/) library, which provides multiple components which use [Material design](https://material.io/design/).


## AngularDart codebase
As typical Angular application, it _uses Component based architecture_. Each component consists of three parts - _Dart_ code, where the logic of the component is implemented, specialized _HTML_ code with interpolations of Dart code and _CSS_ code for styles. The components are complemented with _services_, which provide logic that is not bound to any component and _models_ which represent data.


### Services
The logic which is used across multiple components, such as communication with backend, is implemented in services. These can be found in directory _lib/src/services/_. 

#### Http Services
[HttpServiceBase](../../frontend/lib/src/services/base/http_service_base.dart) is a wrapper of Dart's [http package](https://pub.dev/packages/http), which is implementation of HTTP protocol. This service adds an option, to specify IP address and prefix of all HTTP requests. It adds two functionalities to the http package:
 * Checks, whether the HTTP response was successful (i.e. returned status code indicating success), or throws [HttpException](../../frontend/lib/src/services/base/exceptions.dart),
 * Deserializes and returns the JSON body of the response in form of Dart's Map object
 
[HttpServiceBasicAuthBase](../../frontend/lib/src/services/base/http_service_basic_auth_base.dart) extends [HttpServiceBase](../../frontend/lib/src/services/base/http_service_base.dart) by adding _Authorization_ header to all the HTTP requests, using _Basic authorization token_. This token can be specified directly, or by providing the service with username and password.

#### Local Storage Service
[LocalStorageService](../../frontend/lib/src/services/local_storage_service.dart) is a simple utility service which handles storing and loading of user credentials to the browser's local storage. 

#### Socneto Data Services
[SocnetoDataService](../../frontend/lib/src/services/socneto_data_service.dart) extends [HttpServiceBasicAuthBase](../../frontend/lib/src/services/base/http_service_basic_auth_base.dart) with concrete HTTP requests to SOCNETO backend and returns resulting JSON bodies deserialized to Dart objects. 

The address of the backend is read from the environment variables using utility class [Config](../../frontend/lib/src/config.dart). If it is not specified in the environment variables, it defaults to _localhost:6010_. 

[SocnetoMockDataService](../../frontend/lib/src/services/socneto_mock_data_service.dart) uses the same interface as [SocnetoDataService](../../frontend/lib/src/services/socneto_data_service.dart), but instead of sending requests to the backend, it returns mock data. This service is used only during development.

[SocnetoService](../../frontend/lib/src/services/socneto_service.dart) is a slight wrapper on top of [SocnetoDataService](../../frontend/lib/src/services/socneto_data_service.dart). It uses [LocalStorageService](../../frontend/lib/src/services/local_storage_service.dart) to store user's credentials in the browser's local storage.

#### Platform Status Service
[PlatformStatusService](../../frontend/lib/src/services/platform_status_service.dart) is used to get the status of core SOCNETO components and inform subscribed components about changes in the status of these components. If any component is subscribed for the changes, the service polls backend using [SocnetoService](../../frontend/lib/src/services/socneto_service.dart) every 5 seconds and asks about the platform status. If a different status is returned then in the previous request, then all the subscribed components are informed about this change.


### Components
Most of the frontend codebase consists of implementations of multiple different components. These components can be found in directory _lib/src/components_. As mentioned at the beginning, angular components are typically implemented in three parts (dart, html and css). The convention in this project is, that each part is implemented different file. All of these three files are then placed in the same directory, have the same name and differ only in their extension. 

In this project, instead of using CSS for styles, we use SCSS, which has more extensive syntax. 

#### Shared components
Components designed for reuse in multiple other components are in _lib/src/components/shared_ directory. These components are:
 * [ComponentSelect](../../frontend/lib/src/components/shared/component_select/components_select_component.dart) gets a list of SOCNETO components on input and displays the user list of these components. The user can select any of these components, and after each selection change, this component triggers an event,
 * [Paginator](../../frontend/lib/src/components/shared/paginator/paginator_component.dart) is a simple visualisation of pagination. It provides the user with a list of available pages and triggers an event on each page change,
 * [PlatformStartupInfo](../../frontend/lib/src/components/shared/platform_startup_info) displays the user warning for each core SOCNETO component which is not running. It subscribes itself to the [PlatformStatusService](../../frontend/lib/src/services/platform_status_service.dart) to get notified on each component status changed. This component is used only during startup! After all the components are running, the component unsubscribes itself from the [PlatformStatusService](../../frontend/lib/src/services/platform_status_service.dart), 
 * [SocnetoComponentStatus](../../frontend/lib/src/components/shared/socneto_component_status/socneto_component_status_component.dart) gets a [ComponentStatus](../../frontend/lib/src/models/PlatformStatus.dart) as an input and displays it in a form of a colored icon,
 * [Verification](../../frontend/lib/src/components/shared/verification/verification_modal.dart) is a modal dialogue, which is given a verification message on input, displays the message to the user and triggers an event when the user presses the _yes_ or _no_ button.
 
#### Root Component
 * [AppComponent](../../frontend/lib/src/components/app_component/app_component.dart) is the root component of all the other components. Its only functionality is to check, whether there are valid credentials in the browser's local storage and login the user automatically if there are. Its only child is [AppLayoutComponent](../../frontend/lib/src/components/app_component/app_layout/app_layout_component.dart),
 * [AppLayoutComponent](../../frontend/lib/src/components/app_component/app_layout/app_layout_component.dart) creates the main layout of the page. It renders a top header bar of the page, a hideable drawer on the left and the content below the header. The content can be either the [LoginComponent](../../frontend/lib/src/components/app_component/app_layout/login/login_component.dart) or [WorkspaceComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/workspace_component.dart). Which component should be displayed is based on [current page's url](https://angulardart.dev/tutorial/toh-pt5).
 
#### Login 
The page is composed of a single [LoginComponent](../../frontend/lib/src/components/app_component/app_layout/login/login_component.dart). It provides the user with inputs for login and password. It also disables the inputs during the platform startup, while not all of the core SOCNETO components are not running.

#### Workspace
The user's workspace when logged in is composed of multiple components. The layout of these components and which components are displayed is controlled by [WorkspaceComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/workspace_component.dart). It displays [JobsListComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_list/job_list_component.dart) at the left side all the time, and a second component on the right based on [current page's url](https://angulardart.dev/tutorial/toh-pt5), which can be either [QuickGuideComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/quick_guide/quick_guide_component.dart) or [JobDetailComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/job_detail_component.dart).

The _JobsListComponent_ displays [paginated](../../frontend/lib/src/components/shared/paginator/paginator_component.dart) list of all the user created jobs along with minimalistic info about them // TODO: a subject to change. It is implemented using [material list component](https://dart-lang.github.io/angular_components/#/material_list). The list is retrieved from backend. It also displays a button for stopping a job for each running job, a button to create a new job and the number of running jobs. When pressing the _create new job_ button, the [CreateJobModal](../../frontend/lib/src/components/app_component/app_layout/workspace/job_list/create_job/create_job_modal.dart) is displayed. The modal contains two-step [material stepper component](https://dart-lang.github.io/angular_components/#/material_stepper). The first step contains inputs for each required field in a job definition and the second optional step contains credentials for the job. Submitting a filled job definition is done via backend again.

The _QuickGuideComponent_ displays a simple wizard-like guide for the user. The guide is implemented using [material stepper component](https://dart-lang.github.io/angular_components/#/material_stepper).

##### Job detail
[JobDetailComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/job_detail_component.dart) displays detail of the currently selected job in the [JobsListComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_list/job_list_component.dart). It is a [tabbed container](https://dart-lang.github.io/angular_components/#/material_tab) with two tabs. The first tab contains [ChartsBoardComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/charts_board/charts_board_component.dart) and the second [PostsListComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/posts_list/posts_list_component.dart).

[ChartsBoardComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/charts_board/charts_board_component.dart) expects a _jobId_ on the input, for which the detail should be displayed. It displays [CreateChartButtonComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/charts_board/create_chart_button_component.dart), which when clicked opens [CreateChartModal](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/charts_board/create_chart_modal_component.dart). It also contains one [ChartComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/charts_board/chart/chart_component.dart) for each chart definition from list of all user defined charts, which is retrieved from backend. The [ChartComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/charts_board/chart/chart_component.dart) then queries chart data according to given chart definition from backend, and displays the data in a chart. The chart type is selected from the chart definition.

[PostsListComponent](../../frontend/lib/src/components/app_component/app_layout/workspace/job_detail/posts_list/posts_list_component.dart) contains [paginated](../../frontend/lib/src/components/shared/paginator/paginator_component.dart) list of posts retrieved by a given job. The list is retrieved from backend. It also displays optional filters, which can be applied to the list. It also contains a button to export all the (filtered) posts to CSV, which is retrieved from backend.

### Routes
Some of the components use [angular routing](https://angulardart.dev/tutorial/toh-pt5) to select which component to display. All of the available routes are defined in [routes.dart](../../frontend/lib/src/routes.dart), along with their route parameters and mappings of concrete routes to concrete components.


### Styles
As already mentioned, instead of using plain CSS, we use SCSS. Since web browsers support only CSS, all of our SCSS files are compiled to CSS during the build. The compilation is done by _angular_components_ library, and it is specified in [build configuration file](../../frontend/build.yaml).

#### Theming
In order to easily change the primary and background colors of the whole application, we use theming. Each component has its own styles wrapped in a _SCSS mixin_, which has a _theme_ parameter. This parameter contains palettes for primary and background colors. All of these component mixins are then included in the [apply-theme](../../frontend/lib/src/components/app_component/app_component.scss) mixin which passes the _theme_ parameter to each of them. [There](../../frontend/lib/src/components/app_component/app_component.scss)) are then different CSS classes which use this _apply-theme_ mixin with different predefined theme objects, which can be found in [themes.scss](../../frontend/lib/src/style/theming/themes.scss).

Different color palettes are then defined in [palettes.scss](../../frontend/lib/src/style/theming/palettes.scss), which were created according to [material color palette rules](https://material.io/design/color/the-color-system.html#color-usage-palettes). The SCSS functions for retrieving specific colors from color palettes are implemented in [theming.scss](../../frontend/lib/src/style/theming/theming.scss).


### Models
Directory _/lib/src/models/_ contains all of the models used throughout the application. It contains models for objects sent and received by backend as well as models used exclusively only on frontend. Models representing requests contain function `toMap()` which serializes them to generic `Map` object. This can be then easily serialized to JSON string by [HttpServiceBase](../../frontend/lib/src/services/base/http_service_base.dart) and sent to backend. Models representing responses from backend contain function `fromMap(Map data)` which deserializes them from the given generic `Map data` parameter.


### Interoperability
Dart SDK provides means to communicate with native JavaScript code via [js package](https://api.dart.dev/stable/2.7.1/dart-js/dart-js-library.html). This interoperability is required, when we want to use a JavaScript library, which wasn't rewritten to Dart. 

We use this functionality for two JavaScript libraries. One of them is [toastr](https://codeseven.github.io/toastr/), which is used to display notifications, and the second one is our own JavaScript library for displaying charts. To be able to with these libraries, we need to implement interfaces between Dart and them, using the mentioned `js` package. These interfaces can be found in directory _/src/lib/interop/_.

Even with these interfaces, we still need to import those libraries from the resulting HTML code. This is done in [index.html](../../frontend/web/index.html). The _toastr_ library requires _jQuery_ library to function, so that one is also imported.   


## Pure JavaScript codebase
// TODO: toto odovodnenie ma byt asi skor vo vyvojove ako programatorskej doc, ci?
To draw different types of charts, we use [d3js library](https://d3js.org/). This library was not rewritten to Dart and has an extensive API, so it would take considerable effort to create a Dart interface for it using Dart's `js` library. For this reason, we decided that it will be more simple to write the specific charts components in pure JavaScript, using the d3js library. Then we make a library from it and define a simple API which can be easily interfaced to Dart.

### Charts
The [charts.js](../../frontend/lib/src/charts/charts.js) script exposes `Socneto` object, which contains static functions for creating charts with data as their parameters. Implementations of the charts are then contained in separate JavaScript classes and CSS files. 


## Static
All the static image, which are used by the frontend are contained in _/lib/static_ directory.


## Build
The frontend can be built using two different methods. It can be built using Docker or without Docker.  
### Without Docker
To build the frontend without Docker, the following applications are required:
 * Dart SDK, version 2.5.2
 * nodejs, version 8 or more,

### Gulp
For better performance, we don't want to import from HTML all of our JavaScript and CSS files individually. To merge all of them into one JavaScript and one CSS file, we use [gulp tool](https://gulpjs.com/). This tool can be installed only via _npm_, using command `npm install gulp && npm install gulp -g && npm install gulp-concat`. The gulp is configured via [gulpfile.js](../../frontend/gulpfile.js) to do the merging as a default task, so simply running `gulp` command with no parameters and arguments will create the resulting two files, and put them to _/lib/static_ directory.

### Webdev
Next we need to download all the dependencies for Dart, which are specified in [pubspec.yaml](../../frontend/pubspec.yaml). This is handled by running command `pub get`.

The frontend server in Dart applications is started using _webdev_ tool. This tool was already installed in previous step by _pub_. To be able to use the tool from command line, we need to activate it  using command `pub global activate webdev 2.5.1`. Now we can start the server using command `webdev serve`.

Using `webdev serve` without any parameters, we start the server on address `localhost:8080` and compile Dart code to JavaScript using _dartdevc_ compiler, which is more suitable for development then production. If we want to change the port, on which the server runs, we need to start it using command `webdev serve web:<PORT_NMBER>` (replace <PORT_NMBER>, with a required port number). If we are building the Dart web application on production, it is more suitable to use its `dart2js` Dart to JavaScript compiler, which next to other features also e.g. minifies the resulting JavaScript, increasing the performance of our application. This can be achieved by running the above commands with option `--release`.  

### Using Docker
To build the frontend using Docker, we need to have installed Docker on version 18.0.9.7 or more. The repository contains prepared functional [Dockerfile](../../frontend/Dockerfile). This _Dockerfile_ builds the frontend using `dart2js` compiler, and starts the server on port _8080_, accessible from the host machine. To build and run the image, execute the following commands:
```
docker build -t "frontend" .
docker run -i -t "frontend" -p "8080:8080"
```
//TODO: verify these commands
