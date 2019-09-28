# -*- coding: utf-8 -*-
"""
Created on Mon Mar 11 07:30:22 2019

@author: pdoubravova
"""

import pandas as pd
from textblob import TextBlob as tb
import re
from sklearn.metrics import accuracy_score as acscore


def load_csv():
    data = pd.read_csv('datasets/Tweets.csv', delimiter=',')
    return data


# https://www.geeksforgeeks.org/twitter-sentiment-analysis-using-python/
def clean_tweet(tweet):
    """ Utility function to clean tweet text by removing links, special characters
        using simple regex statements.
    """
    return ' '.join(re.sub("(@[A-Za-z0-9]+)|([^0-9A-Za-z \t]) |(\w+:\/\/\S+)", " ", tweet).split())


def compute_sentiment(train):
    raise NotImplementedError


# https://www.geeksforgeeks.org/twitter-sentiment-analysis-using-python/
def get_text_sentiment(tweet):
    """
    Utility function to classify sentiment of passed tweet
    using textblob's sentiment method
    """
    # create TextBlob object of passed tweet text 
    analysis = tb(tweet)
    # set sentiment 
    if analysis.sentiment.polarity > 0:
        return 1
    elif analysis.sentiment.polarity == 0:
        return 2
    else:
        return 0


if __name__ == '__main__':
    print('start')
    data = load_csv()
    data['text'].apply(clean_tweet)
    prediction = data['text'].apply(get_text_sentiment)
    accuracy = acscore(data['airline_sentiment'], prediction)
    print(accuracy)
    print('end')
