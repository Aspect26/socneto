# Backend API

This document contains list of all available requests on backend.

## User

### Login

Verifies whether given username and password are valid 

`GET /api/user/login`

#### Request parameters

**Body**:

```json
{
    "username": "<string>",
    "password": "<string>"
}
```

#### Response

**Codes**:

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The given credentials are valid |
| `401`       | The given credentials are not valid |


**Example**:

```json
{
    "username": "<string>"
} 
```

## Jobs

### Get All

Retrieves all jobs created by user

`GET /api/job/{username}/all`

#### Authorization

Basic authorization

#### Request parameters

**Query parameters**

| Name | Description                    | Example |
| :--- | :----------------------        |:--- |
| `username` | Username of the user whose jobs are to be queried |`admin` |

#### Response

**Codes**

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The requested objects can be found in the body |
| `401`       | User is not authorized to see the given jobs |

**Example**

```json
[
  {
    "jobId": "<guid>",
    "jobName": "<string>",
    "status": "<enum>",
    "startedAt": "<datetime>",
    "finishedAt": "<datetime>"
  },
  {
    "jobId": "<guid>",
    "jobName": "<string>",
    "status": "<enum>",
    "startedAt": "<datetime>",
    "finishedAt": "<datetime>"
  }
]
```

### Job status

Retrieves current job status

`GET /api/job/{jobId}/status`

#### Authorization

Basic authorization

#### Request parameters

**Query parameters**

| Name | Description                    | Example |
| :--- | :----------------------        |:--- |
| `jobId` | GUID of the job whose status is to be queried | `09cb74d9-1fad-47ed-9af8-5f90f01a1a34` |

#### Response

**Codes**

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The requested status can be found in the body |
| `401`       | User is not authorized to see the given job |

**Example**
```json
{
  "jobId": "<guid>",
  "jobName": "<string>",
  "status": "<enum>",
  "startedAt": "<datetime>",
  "finishedAt": "<datetime>"
}
```

### Job posts

Retrieves all the posts the job has acquired 

<!-- TODO this query may change later -->

### Aggregation analysis

Get aggregation type analysis for given job

`GET /api/job/{jobId}/aggregation_analysis`

#### Authorization

Basic authorization

#### Request parameters

**Query parameters**

| Name | Description                    | Example |
| :--- | :----------------------        |:--- |
| `jobId` | GUID of the job whose analysis is to be queried | `09cb74d9-1fad-47ed-9af8-5f90f01a1a34` |

**Body**
```json
{
    "analyser_id": "<string>",
    "analysis_property": "<string>"
}
```

#### Response

**Codes**

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The requested analysis can be found in the body |
| `401`       | User is not authorized to see the given job |

**Example**
<!-- TODO aggregation analysis response example -->

### Array analysis

Get array type analysis for given job

`GET /api/job/{jobId}/array_analysis`

#### Authorization

Basic authorization

#### Request parameters

**Query parameters**

| Name | Description                    | Example |
| :--- | :----------------------        |:--- |
| `jobId` | GUID of the job whose analysis is to be queried | `09cb74d9-1fad-47ed-9af8-5f90f01a1a34` |

**body**
```json
{
    "analyser_id": "<string>",
    "analysis_properties": "[<string>]"
}
```

#### Response

**Codes**

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The requested analysis can be found in the body |
| `401`       | User is not authorized to see the given job |

**Example**
<!-- TODO array analysis response example -->

## Components

Contains requests on SOCNETO platform components

### Analysers

Gets all analysers registered to the platform

`GET /api/components/analysers`  

#### Authorization

None

#### Request parameters

None

#### Response

**Codes**

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The requested objects can be found in the body |

**Example**
```json
[
  {
    "identifier": "<string>",
    "analysisProperties": {
      "<string>": "<enum>",
      "<string>": "<enum>"
    },
    "componentType": "<enum>"
  },
  {
    "identifier": "<string>",
    "analysisProperties": {
      "<string>": "<enum>",
      "<string>": "<enum>"
    },
    "componentType": "<enum>"
  }
]
```

### Data acquirers

Gets all data acquirers registered to the platform

`GET /api/components/acquirers`  

#### Authorization

None

#### Request parameters

None

#### Response

**Codes**

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The requested objects can be found in the body |

**Example**
```json
[
  {
    "identifier": "<string>",
    "componentType": "<enum>" 
  },
  {
    "identifier": "<string>",
    "componentType": "<enum>" 
  },
  {
    "identifier": "<string>",
    "componentType": "<enum>" 
  }
]
```

## Charts

Contains requests for user specified charts

### Get all

Gets all user defined charts for a given job

`GET /api/charts/{jobId}`

#### Authorization

Basic authorization

#### Request parameters

**Query parameters** 

| Name | Description                    | Example |
| :--- | :----------------------        |:--- |
| `jobId` | GUID of the job whose charts are to be queried | `09cb74d9-1fad-47ed-9af8-5f90f01a1a34` |

#### Response

**Codes**

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The requested charts can be found in the body |
| `401`       | User is not authorized to see the given job |

**Example**
```json
[
  {
    "analysis_data_paths": [
      {
        "property": {
          "identifier": "<string>",
          "type": "<enum>"
        },
        "analyser_id": "<string>"
      }
    ],
    "chart_type": "<enum>",
    "is_x_datetime": "<boolean>"
  }
]
```

### Create 

Creates a new chart for a given job

`GET /api/charts/{jobId}/create`

#### Authorization

Basic authorization

#### Request parameters

**Query parameters** 

| Name | Description                    | Example |
| :--- | :----------------------        |:--- |
| `jobId` | GUID of the job for which to create the chart | `09cb74d9-1fad-47ed-9af8-5f90f01a1a34` |

**Body**

```json
{
  "analysis_data_paths": [
    {
      "analyser_component_id": "<string>",
      "analyser_property": {
        "identifier": "<string>",
        "type": "<enum>"
      }   
    },
    {
      "analyser_component_id": "<string>",
      "analyser_property": {
        "identifier": "<string>",
        "type": "<enum>"
      } 
    }
  ],
  "chart_type": "<enum>",
  "is_x_post_datetime": "<boolean>"
}
```

#### Response

**Codes**

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The chart was created |
| `401`       | User is not authorized to see the given job |

**Example**
```json
{
  "success": "<boolean>"
}
```