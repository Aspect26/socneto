
Api allows user to 
- manage jobs
- manage users
- query results

### Job Management Api

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

`GET /api/user/{userId}/jobs`

... specify once the api is stable


### Results

`GET /api/job/{jobId}/result`

... specify once the api is stable
