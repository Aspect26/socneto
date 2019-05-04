import pandas as pd
from textblob import TextBlob as tb
import re
from sklearn.metrics import accuracy_score as acscore
import pokusAnalyza as analysis

"""
Module for computing sentiment.
"""

def get_sentiment(text):
    """Method for evaluating one given text."""
    return analysis.get_tweet_sentiment(text)



def get_sentiments(item_dict):
    """Method for coputing sentiment of dictionary of texts."""
    result = {}
    for key, value in item_dict.items():
        result[key] = get_sentiment(value)
    return result
