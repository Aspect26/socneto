# -*- coding: utf-8 -*-
"""
Created on Mon Mar 11 07:30:22 2019

@author: pdoubravova
"""

import pandas as pd
import re
from sklearn.metrics import accuracy_score as acscore
import fasttext


file = ... #TODO cesta k modelu
model = fasttext.load_model(file)


# https://www.geeksforgeeks.org/twitter-sentiment-analysis-using-python/
def get_text_sentiment(tweet):
    """
    Returns sentiment and its probability for given string.
    label: '__label__positive', '__label__negative', '__label__neutral'
    """
    label, probab = model.predict(tweet)
    return label[0], probab[0]




# if __name__ == '__main__':
#
#     print('start')
#     l,p = get_text_sentiment("super nice wrong day")
#     print('end')
