# -*- coding: utf-8 -*-
"""
Created on Mon Mar 11 07:30:22 2019

@author: pdoubravova
"""

import pandas as pd
import textblob as tb
import re

def load_csv():
    data = pd.read_csv('datasets/Tweets.csv',delimiter = ',')
    return data

#https://www.geeksforgeeks.org/twitter-sentiment-analysis-using-python/
def clean_tweet(tweet): 
        ''' 
        Utility function to clean tweet text by removing links, special characters 
        using simple regex statements. 
        '''
        return ' '.join(re.sub("(@[A-Za-z0-9]+)|([^0-9A-Za-z \t]) |(\w+:\/\/\S+)", " ", tweet).split()) 
    
def compute_sentiment(train):
    

#https://www.geeksforgeeks.org/twitter-sentiment-analysis-using-python/
def get_tweet_sentiment(self, tweet): 
    ''' 
    Utility function to classify sentiment of passed tweet 
    using textblob's sentiment method 
    '''
    # create TextBlob object of passed tweet text 
    analysis = tb(self.clean_tweet(tweet)) 
    # set sentiment 
    if analysis.sentiment.polarity > 0: 
        return 'positive'
    elif analysis.sentiment.polarity == 0: 
        return 'neutral'
    else: 
        return 'negative'

if __name__ == '__main__':
    print('start')
    data = load_csv()
    
    