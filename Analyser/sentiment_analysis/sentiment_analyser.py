import sentiment_analysis.Analysis as a

"""
Module for computing sentiment.
"""

def get_sentiment(text):
    """Method for evaluating one given text."""
    return a.get_text_sentiment(text)



def get_sentiments(item_dict):
    """Method for coputing sentiment of dictionary of texts."""
    result = {}
    for key, value in item_dict.items():
        result[key] = get_sentiment(value)
    return result
