# How to use custom dataset in Socneto

Socneto allows users to use their own dataset as long as they comply with requirements.

## Dataset requirements

- File format: `csv` or `json`. In case of json, the file is assumed to be file were each line is a record json called. This format is know as json lines but it will be referred to as plain `json`.
- Content must be transformable into an entity `Post`. The transformation is done via mapping file accompanying the dataset.

## Dataset transformation

The components processing data from custom dataset expects each record to have the following fields:

| Column name    | Description                                               | Example Value             |
| :------------- | :-------------------------------------------------------- | :------------------------ |
| originalPostId | The unique post string identifier                         | 1234abcd-efgh             |
| text           | The text of the given post                                | This is example post text |
| language       | The language cod of the post [^1]                         | "en","cs" or null         |
| source         | A field prefilled by the Data acquirer                    | twitter_acquirer          |
| authorId       | The name or id of the author                              | author_123 or null        |
| dateTime       | Date time when the post was created in the ISO8601 format | 2020-01-03T23:42:53       |

[^1]: The post be in any language, but only sentiment of english posts can be analysed

Socneto implements basic automatic transformation that maps records from the dataset to the `Post` format. The transformation requires a json configuration file specifying 

- Mapping of fields from dataset records to `Post`
- Specification of fixed values (e.g. language may be the same across the dataset or the value is not present)
- File format (either `csv` or `json`)
- Format specific configuration



### Json mapping file example

```json
{
    "dataFormat":"json",
    "mappingAttributes":{
        "hasHeaders":true,
        "dateTimeFormatString":"yyyy'-'MM'-'dd'T'HH':'mm':'ss",
        "fixedValues":{
            "language":"en"
        },
        "elements":{
            "originalPostId":"PostId",
            "text":"Text",
            "authorId":"UserId",
            "dateTime":"PostDateTime"
        }
    }
}
```

This file is used to map the following json record:

```json
{
    "PostId":"1195555506847744",
    "Text":"The movie xyz was excelent",
    "UserId":"abc",
    "PostDateTime":"2019-11-16T18:48:03"}
```

The resulting `Post` (shown in json) is the following:
```json
{
    "originalPostId":"1195555506847744",
    "text":"The movie xyz was excelent",
    "source":"twitter_acquirer", // filled with the acqurier
    "authorId":"abc",
    "dateTime":"2019-11-16T18:48:03",
    "language":"en"
}
```


### Csv mapping file example
```json
{
    "dataFormat":"csv",
    "mappingAttributes":{
        "hasHeaders":true,
        "dateTimeFormatString":"ddd MMM dd HH:mm:ss PDT yyyy",
        "fixedValues":{
            "language":"en"
        },
        "indices":{
            "originalPostId":1,
            "text":5,
            "authorId":4,
            "dateTime":2
        }
    }
}
```

This mapping file is used to map the following record (in csv format):


| target| id          | date        | flag        | user        | text     |
|:---|:---|:---|:---|:---|:---|
| 0                            | abc087                         | Sat May 16 23:58:44 UTC 2009 | xyz                          | robotickilldozr       | xyz is fun |

The resulting `Post` (shown in json) is the following:
```json
{
    "originalPostId":"abc087",
    "text":"xyz is fun",
    "source":"twitter_acquirer", // filled with the acqurier
    "authorId":"abc",
    "dateTime":"2009-05-16T23:58:44",
    "language":"en",  
}
```

The example mappings has the following  fields:

| Field name| Description          | Required        | 
|:---|:---|:---|:---|
|`dataFormat`|Format of the dataset. `json` or `csv`|yes|
|`mappingAttributes`|Root element with mappings and element specific values|yes|
|`fixedValues`| A map where a key is a name of a field of the `Post` entity and the value is a text of the field | no |
|`indices`| ( For `csv` dataFormat only) A map where a key is  a name of a field of the `Post` entity and value is an index of the record with target value | `csv` only |
|`elements`| ( For `json` dataFormat only) A map where a key is  a name of a field of the `Post` entity and value is a name of an element of the record with target value. Mapping to names of csv columns is not supported, use `indices` instead.| `json` only|
|`hasHeaders`| Indication whether the `csv` file contains headers or not (they will be skipped if true)| `csv` only|
|`dateTimeFormatString`| If dateTime mapping is specified, then this field is used as mask for custom date time formats. If no format string is specified then [the default one](https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tryparse?view=netcore-3.1) is used. | no |

**CAUTION**: When parsing of the date time is not successful, then the `dateTime` is set to `0001-01-01T00:00:00`.

**NOTE**: Mappings does not support any nested json files. If user needs so, then there is a possibility to extend the system with own implementation supporting such a mapping.

When transformation fails to produce valid `Post` object then the object is not produced and will be ignored. User is noticed about the problem with a warning message that can be found in the logs.


## Files used

The following text works with [this dataset](https://www.kaggle.com/kazanova/sentiment140) that can be found on [kaggle.com](https://www.kaggle.com).

The dataset will be referred to as `tweets.csv`. It is a single `csv` file with 1.6 million records with the following columns

| Column name | Description                                                                                             | Example Value                |
| :---------- | :------------------------------------------------------------------------------------------------------ | :--------------------------- |
| target      | The polarity of the tweet e.g. 0 = negative, 2 = neutral, 4 = positive                                  | 0                            |
| id          | The id of the tweet                                                                                     | 2087                         |
| date        | The date of the tweet                                                                                   | Sat May 16 23:58:44 UTC 2009 |
| flag        | The query which was used to obtains the given tweet. If there is no query, then this value is NO_QUERY. | xyz                          |
| user        | The user that tweeted                                                                                   | robotickilldozr              |
| text        | The content of the tweet                                                                            | xyz is fun |

The mapping used (referred to as `tweets.mapping`) is the same as was presented in the [csv mapping file example](#csv-mapping-file-example) e.g.:

```json
{
    "dataFormat":"csv",
    "mappingAttributes":{
        "hasHeaders":true,
        "dateTimeFormatString":"ddd MMM dd HH:mm:ss PDT yyyy",
        "fixedValues":{
            "language":"en"
        },
        "indices":{
            "originalPostId":1,
            "text":5,
            "authorId":4,
            "dateTime":2
        }
    }
}
```

## Steps

The following steps leads to successfully feeding Socneto with custom data.

### Step 1 - Upload files

- Connect to `http://acheron.ms.mff.cuni.cz:39107`
- Type in the following credentials 
```
USER: TODO
Password: TODO
```

- Hit red plus button and create bucket `movie-tweets`
- Hit upload button and upload `tweets.csv` and `tweets.mapping` to that bucket. The result should look like this

![Upladed files][uploaded]

<!-- Images -->
[uploaded]: images/guide-custom-dataset/minio-files-in-place.PNG "Upladed"

### Step 2 - Create job

TODO UI












