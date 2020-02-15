# -*- coding: utf-8 -*-
"""
Created on Mon Mar 11 07:30:22 2019

@author: pdoubravova
"""

import pandas as pd
import re
import ktrain

class Analysis:
    def __init__(self, file):
        self.model = ktrain.load_predictor(file)

        super().__init__()

    # https://www.geeksforgeeks.org/twitter-sentiment-analysis-using-python/
    def get_text_sentiment(self, tweet):
        """
        Returns sentiment and its probability for given string.
        label: '__label__positive', '__label__negative', '__label__neutral'
        """

        probab = self.model.predict_proba(tweet)
        label = probab.argmax()
        num_label = {
            1:1,
            2:-1,
            0:0
        }[label]

        return num_label, probab[label]




# if __name__ == '__main__':
#
# print('start')
# l,p = Analysis('./distilbert_predictor').get_text_sentiment("super nice wrong day")
# print(l)
# print(p)
# print('end')
