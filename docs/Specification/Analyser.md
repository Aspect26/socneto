## Implemented problems
* Topic modeling
* Setiment analysis
* Langugage detection

We will implement this analysis for czech and english. It should be easily exensible to other languages.

### Data
Big amount of data will be needed for NLP tasks. We do not assume anotating any data, but this will be one of bottleneck of our model performance. 
There are data sets available for english both social nework posts and other short format data (like IMDb). There exists some data for czech too, but they are smaller.

There is need to prepare some train and test data sets - to gather data, clean them, divide into sets.

### Topic modeling (TM)
This is generally a difficult problem for both languages.  
Possible approaches :
* Word frequency + dictionary - quite easy approach, but it can't handle i.e. pronoun references
* Entity linking - is needed to use some knowledge base
* Entity modeling  - knowledge base needed
* Some probabilistic models (Latent Dirichlet Allocation - LDA)

Combination of different approaches will be used:
frequency, LDA and maybe entity linking, as for czech existing knowledge base is not so large. We will use existing libraries and knowledge bases.

Topics for hierarchiacal structures like posts + their comments will be find for post and related comments together.

### Language detection (LD)
Easier than general language detection problem, because we have just three categories  - czech, english, other

We will use existing model library (probably pycld)

With usage of BERT, language decetion will not be needed for SA.

### Sentiment analysis (SA)
Quite subjective task.
OUtputs:
* polarity
* subjectivity

Using BERT:
https://github.com/google-research/bert/blob/master/predicting_movie_reviews_with_bert_on_tf_hub.ipynb


### Analyzer output
The analyzers will process each post they receive from the acquirers. After analyzing a post they will send the result to the storage. Because these analyses will be used by other components (mainly frontend), the analyzers' output will need to be in some standardized form. As this output will be mainly processed by a computer, we decided to use a computer friendly JSON format.

The structure of the JSON will need to be robust and simple enough, so that the user of frontend may easily specify which data he wants to visualize using JSONPath. The structure of the output is following:
```json
{
    "analyzer_name": {
        "analysis_1": { "type": "number", "value": 0.56 },
        "analysis_2": { "type": "string", "value": "rainbow" },
        "analysis_3": { "type": "[string]", "value": ["friends", "games", "movies" ] }
    }
}
```
The whole analysis is packed in one object named after the analyzer. As the analyzer may compute multiple analyses at once, each one will be also represented by one object named after the analysis. The object representing the analysis has a strict format. It contains exactly two attributes:
 * *type*: specifying the type of the result. The supported types will be *number* (including integers and floating point values), *string* and *lists* of these two. Lists of lists will *not* be supported,
 * *value*: the actual result of the analysis

There may be multiple analyzers in our framework, so all their outputs are inserted into one analysis object. We don't use arrays of objects but named objects instead, so that the user may easily specify analyzer and analysis in JSONPath when creating a new chart on frontend.

Here we provide an example of a post's complete analysis. It contains analyses from two analyzers - *keywords* and *topic*. Keywords analyzer returns one analysis called *top-10* where the values are found keywords in the post. The topic analyzer returns one analysis, *accuracy*, specifying how much the post corresponds to the given topic.
```json
{
    "keywords": {
        "top-10": {
            "type": "[string]",
            "value": [
                "Friends", "Games", "Movies", ...
            ]
        }
    },
    "topic": {
        "accuracy": {
            "type": "number",
            "value": 0.86
        }
    },
}
```
