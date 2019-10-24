# coding=utf-8
import random

import pandas as pd
from sklearn.utils import shuffle
from language_recognizer import detect_language

data_czech = pd.read_csv("/Users/petravysusilova/Documents/School/SWProject/SWProject/data/Datasets/facebook/gold-posts.txt",
                   delimiter='\n', names=[5])
data_en = pd.read_csv("/Users/petravysusilova/Documents/School/SWProject/SWProject/data/Datasets/trainingandtestdata/training.1600000.processed.noemoticon.csv",
                   delimiter=',', header=None, usecols=[5])

data_czech['lang'] = 1
data_en['lang'] = 2

merged = pd.concat([data_czech, data_en])
merged = merged.sample(frac=1, random_state=42)

acc = 0
for index,i in merged.iterrows():
    text = i[5]
    lang = i['lang']
    pred = detect_language(text)
    if pred != lang:
        acc += 1

print(acc)
'''
Presnost na testovacím datasetu je 0.9592416149
Pocet radku: 1610000
Chybne urceno: 65621
'''



