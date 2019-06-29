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
    "version":"v1.23",
}
```
Topic: `job_management.registration.request`

- 