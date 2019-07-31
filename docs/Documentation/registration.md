request

```json
{
    "componentType":"<on of the following: Analyser,DataAcquirer,Storage>",

    "componentId":"<componentId>",
    "inputChannelName":"job_management.component_data_input.<componentId>",
    "updateChannelName":"job_management.job_configuration.<componentId>",
    // not in the PoC
    //"sample_output":"foo_bar"
}
```

This request must be sent to a topic `job_management.registration.request` before any communication begin.

Then job notification are pushed:

Data acquisition job config update

```json
{
     "jobId":"<guid>",
     "attributes":{         
         "TopicQuery":"some query - some other query"
         // possibly some other attributes
     },        
    "outputMessageBrokerChannels":["channel1","channel2"]
}
```

Analyser job config update
```json
{
     "jobId":"<guid>",
     "attributes":{         
         // possibly some attributes
     },        
    "outputMessageBrokerChannels":["channel1","channel2"]
}
```

Storage job config update
```json
{
     "jobId":"<guid>",
     "attributes":{         
         // possibly some attributes
     }
}
```