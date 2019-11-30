# -*- coding: utf-8 -*-
"""
Created on Mon Mar 11 07:30:22 2019

@author: pdoubravova
"""

import pandas as pd
import re
import fasttext

class Analysis:
    def __init__(self, file):
        self.model = fasttext.load_model(file)

        super().__init__()

    # https://www.geeksforgeeks.org/twitter-sentiment-analysis-using-python/
    def get_text_sentiment(self, tweet):
        """
        Returns sentiment and its probability for given string.
        label: '__label__positive', '__label__negative', '__label__neutral'
        """
        label, probab = self.model.predict(tweet)
        label = label[0]
        num_label = {
            '__label__positive':1,
            '__label__negative':-1, 
            '__label__neutral':0
        }[label]

        return num_label, probab[0]




# if __name__ == '__main__':
#
#     print('start')
#     l,p = get_text_sentiment("super nice wrong day")
#     print('end')
