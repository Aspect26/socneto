request

```json
{
    "componentType":"<on of the following: Analyser,DataAcquirer,Storage>",

    "componentId":"<componentId>",

    "updateChannelName":"job_management.job_configuration.<componentId>",

    "sample_output":"foo_bar"
}
```

This request must be sent to a topic `job_management.registration.request` before any communication begin.

