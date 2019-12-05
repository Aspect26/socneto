# Storage API

## Components

Endpoint used to track all system components.

<!-- Used by JMS upon registration -->

### Get all components

Returns all registered components

`GET /components`

#### Request parameters

None.

#### Response

**Codes**:

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The requested objects can be found in the body |

<!-- TODO rest -->

**Example**:

```json
[{
    "id": "<string>",
    "type": "<string>",
    "inputChannelName": "<string>",
    "updateChannelName": "<string>",
    "attributes": "json"
},    
{
    "id": "<string>",
    "type": "<string>",
    "inputChannelName": "<string>",
    "updateChannelName": "<string>",
    "attributes": "json"
}]
```

### Get component by id

`GET /components/{id}`

#### Request parameters

**Query parameter**:

| Name | Description                    | Example |
| :--- | :----------------------        |:--- |
| `id` | String id of a given component |`DataAcquirer_twitter` |

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `200`       | Request was successful. The requested object can be found in the body |
| `404`       | Object with the given id was not found                                |

<!-- TODO rest -->

**Example**:

```json
{
    "id": "<string>",
    "type": "<string>",
    "inputChannelName": "<string>",
    "updateChannelName": "<string>",
    "attributes": "json"
}
```

### Insert component

`POST /components`

#### Request parameters

**Query parameter**:

| Name | Description             | Example |
| :--- | :---------------------- | :--- |
| `id` | Id of a given component | `DataAcquirer_twitter` |

**Body**:

```json
{
    "id": "<string>",
    "type": "<string>",
    "inputChannelName": "<string>",
    "updateChannelName": "<string>",
    "attributes": "json"
}
```

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `204`       | Request was successful |

<!-- TODO rest -->

**Example**: 

None.

### Update component

`PUT /components/{id}`

#### Request parameters

| Name | Description             | Example |
| :--- | :---------------------- |:---|
| `id` | Id of a given component |`DataAcquirer_twitter` |

**Body**:
```json
{
    "id": "<string>",
    "type": "<string>",
    "inputChannelName": "<string>",
    "updateChannelName": "<string>",
    "attributes": "json"
}
```

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `204`       | Request was successful |

<!-- TODO rest -->

**Example**: 

None.

## Job

Used to keep track job.

### Get all user's job

Returns all jobs belonging to the given user

`GET /jobs`

#### Request parameters

None.

#### Response

**Codes**:

| Status code | Description                                                            |
| :---------- | :--------------------------------------------------------------------- |
| `200`       | Request was successful. The requested objects can be found in the body |

<!-- TODO rest -->

**Example**:

```json
[{
        //TODO
}]
```


### Get job by id

`GET /jobs/{id}`

#### Request parameters

**Query parameter**:

| Name | Description                    | Example |
| :--- | :----------------------        |:--- |
| `id` | String id of the given job      |`d1bb2d80-0cd0-4391-b9d3-e317def0ec58` |

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `200`       | Request was successful. The requested object can be found in the body |
| `404`       | Object with the given id was not found                                |

<!-- TODO rest -->

**Example**:

```json
{
    // TODO
}
```

### Insert a job

`POST /jobs`

#### Request parameters

**Query parameter**:

| Name | Description             | Example |
| :--- | :---------------------- | :--- |
| `id` | Id of the given job | `d1bb2d80-0cd0-4391-b9d3-e317def0ec58` |

**Body**:

```json
{
    // TODO
}
```

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `204`       | Request was successful |

<!-- TODO rest -->

**Example**: 

None.

### Update a job

`PUT /jobs/{id}`

#### Request parameters

| Name | Description             | Example |
| :--- | :---------------------- |:---|
| `id` | Id of the given job |`d1bb2d80-0cd0-4391-b9d3-e317def0ec58` |

**Body**:
```json
{
   // job
}
```

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `204`       | Request was successful |

<!-- TODO rest -->

**Example**: 

None.


## Component's job configurations

Each submitted job uses multiple components. Each component is configured differently according to the user needs.  

<!-- This endpoint is used when a component restarts... it needs to know what jobs to start doing again. -->

### Get all component's job configurations for the given component

<!-- TODO: Notice that the uri is different -->

`GET /components/{componentId}/configs`

#### Request parameters

**Query parameter**:

| Name | Description                    | Example |
| :--- | :----------------------        | :--- |
| `componentId` | String id of the component | `DataAcquirer_twitter` |

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `200`       | Request was successful. The requested object can be found in the body |
| `404`       | Object with the given id was not found                                |

<!-- TODO rest -->

**Example**:

```json
[
    {
    "jobId": "string",
    "outputChannelNames": ["string"],
    "attributes": "json"
    },
    {
    "jobId": "string",
    "outputChannelNames": ["string"],
    "attributes": "json"
    }
]
```

### Insert component config

POST
<!-- TODO question. Why there is no insert? -->

<!-- 
 "JobStorageOptions": {
      "BaseUri": "http://localhost:8888",
      "AddJobRoute": "jobs",
      "AddJobConfigRoute": "jobs/{0}/config",
      "UpdateJobRoute": "jobs",
      "GetJobRoute": "jobs"
    }, -->


## Component's jobs metadata



### Get component's job's metadata by id

`GET /components/{componentId}/metadata/job/{jobId}`

#### Request parameters

**Query parameter**:

| Name | Description                    | Example |
| :--- | :----------------------        |:--- |
| `componentsId` | String id of the given component |`DataAcquirer_twitter` |
| `jobId` | String id of the given job |`d1bb2d80-0cd0-4391-b9d3-e317def0ec58` |

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `200`       | Request was successful. The requested object can be found in the body |
| `404`       | Object with the given id was not found                                |

<!-- TODO rest -->

**Example**:

```json
{
    "property_x":"value",
    ...
}
```

### Insert a component's job's metadata

`POST /components/{componentId}/metadata`

#### Request parameters

**Query parameter**:

| Name | Description             | Example |
| :--- | :---------------------- | :--- |
| `componentId` | Id of a given component | `DataAcquirer_twitter` |

**Body**:

```json
{
    "property_z":"value",
    ...
}
```

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `204`       | Request was successful |

<!-- TODO rest -->

**Example**: 

None.

### Update component's job's metadata

<!-- since this is put, should not there be also job id? -->

`PUT /components/{componentId}/metadata`

#### Request parameters

| Name | Description             | Example |
| :--- | :---------------------- |:---|
| `componentId` | Id of the given component |`DataAcquirer_twitter` |

**Body**:
```json
{
    "property_w":"value",
    ...
}
```

#### Response

**Codes**:

| Status code | Description                                                           |
| :---------- | :-------------------------------------------------------------------- |
| `204`       | Request was successful |

<!-- TODO rest -->

**Example**: 

None.
