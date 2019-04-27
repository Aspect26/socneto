POC `SAnalyser` doc draft
===

## preface

This document contains basic info about the poc. The code represents a base for to-be framework for *social network* (SN) data analysis.

## requirements

- .Net core 2.2
- Kafka in order to communicate
  - Topic (`StoreToDbTopic` by default, can be changed in configuration)

## Interaction

It is a .net core web-api application. 

Responds only to https request to `/api/submit` with `Application/json` MIME type. 

example:

```
curl --header "Content-Type: application/json" \
  --request POST \
  --data '{"Topic":"Charles"}' \
  <url>
```

### input

Expects input in following format
~~~json
{
	"Topic":"apple",
}
~~~

### output

Returns id of a job

```json
{
	"jobId":"58d8e8e2-603d-4bcb-af24-5692a0a62126"
}
```
## configuration

To configure the application, see `appsettings.json` in `Coordinator` section of the config. The config can be found at `Socneto/Api` project.

```json
{
	"KafkaOptions": {
      "ProduceDbStoreTopic": "StoreToDbTopic",
      "ServerAddress": "<url>" 
	}
}
```

## Implementation

see documentation **!TODO**
