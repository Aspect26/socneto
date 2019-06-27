## Implemented problems
* Topic modeling
* Setiment analysis
* Langugage detection
We will implement this analysis for czech and english. It should be easily exensible for other languages.

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