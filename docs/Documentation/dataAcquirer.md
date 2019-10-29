Data acquirer
---

Reads data from data source

Supported data sources:

- twitter
- reddit
- file

Supports querying

> TODO define the query language

User can create its own as long as it follows basic component rules (reference to TODO)

Watchdog mode!


## Inputs

> TODO topic name

Data acquirer listens on a topic `<channel>`.
It expects json with the following properties

| Property        | Description|
| :------------- |:-------------|
| `jobId`      | UUID of a given job |
| `command` | In case of a new job `start` or `stop` when job is meant to be stopped. |
| `outputMessageBrokerChannels` | Array of output topics in which DA send all data |
| `attributes` | Free form structure. It contains `topicQuery` (described in TODO). In case of Twitter DA, it also must contain credentials. |

Example

```json
{
    "jobId":"aa7f4c4d-c35c-4f32-a384-472c9f322975",    
    "command":"start",    
    "outputMessageBrokerChannels":["output_channel_1","output_channel_2"],
    "attributes":{
        "topicQuery":"matfyz"
    }
}
```

## Output

Output must be in json and have the following properties

| Property   | Description|
| :----------|:-------------|
| `postId`   | string id of the post processed |
| `jobId`    | Id of the respective job |
| `text`     | Text of the post |
| `source`   | Name of the acquirer which produced the output |
| `authorId` | author name |
| `dateTime` | date in the format `YYYY-MM-DDTHH:mm[.fffff]` |

```json
{
    "postId": "123344121314",
    "jobId": "aa7f4c4d-c35c-4f32-a384-472c9f322975",
    "text": "Matfyz has its own web. Check it out at matfyz.cz",
    "source": "twitter",
    "authorId": "@@tobi@@",
    "dateTime": "2017-10-20T12:34:11",
}
```
## Processing

Data acquirer is valid if it

- register itself - otherwise it can't be accessed
- listens to job submitting on the channel sent in the request 
- outputs post (one-at-a-time) to the channels in a job configuration

**NOTE** The job is expected to be stopped manually by user. In case of social networks it makes sense. It waits for the new post to be published. 

## Implementation of the twitter data acquirer

It register itself

Listens to the jobs

Once any job arrives, getting the data starts. Downloading is handled by a library `LinkToTwitter` (Todo reference) which tackles problems with limits.

When no post is present the service waits for new to arrive until user stops it.
