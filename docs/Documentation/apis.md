
Data acquirer has to produce this
```json

{
    "text":"adfadfasdf",

    "componentId":"fb",

    "authorId":"asdfq2q314kladsf",
    
    "postDateTime":"2008-10-01T17:04:32",

    "jobId":<guid>,

    "postId":<guid>
}
```

analysis
```

```

Otherwise analyser will not accept it



Data acquisition notification look like this
```json
{
    "jobId":<guid>,
    "outputMessageBrokerChannels":["<channel-name>"],
    "topic":"data"
    "attributes":{
        "custom":"data"
    }
}
```

Analyser config notification look like this
```json
{
    "jobId":<guid>,
    "outputMessageBrokerChannel":"<channel-name>",
    "attributes":{
        "custom":"data"
    }
}
```