```
This is going to be part of a documentation


```
## Kafka

TODO disable auto create topics
Naming conventions `context.message_type.data_type`

## Job management service

Registration request

Format:
```
{
    "componentId":"sent_analyser",
    "componentType":"<analyser, dataaquirer>,
    "updateChannelName":"job_management.registration.request"
}
```
Topic: `job_management.registration.request`

- 