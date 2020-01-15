import LDA
import pandas as pd
import os.path
import matplotlib.pyplot as plt
import numpy as np

def analyse_text(lda, text):
    a = lda.get_topic_keywords(text)

    topics =np.array( a[0])
    probs = np.array(a[1])
    zipped = zip(topics.flatten(), probs.flatten())
    dic = {}
    for topic,prob in zipped:
        if topic not in dic:
            dic[topic]=prob
        else:
            dic[topic] = dic[topic] + prob

    return list(dic.items())

def read_data(data_path):
    limit = 1000
    english_only = True
    df = pd.read_json(data_path, lines = True,convert_dates=False,encoding='utf8')
    hd = df.head(limit)
    if english_only:
        hd = hd[(hd['language'] == 'en')]
    return hd

def analyse_dataframe_text(df):
    dic = {}
    for index, row in df.iterrows():
        text = row['text']
        topics = analyse_text(lda,text)
        for t,p in topics:
            if t not in dic:
                dic[t]=1
            else:
                dic[t] = dic[t] + 1
    return list(dic.items())

lda = LDA.LDAAnalysis()
data_path = "D:/data_tw/ces.data"

df = read_data(data_path)
data = analyse_dataframe_text(df)

#topics = ['iot','iiot', 'industry','smart','car']
not_topics = ['las', 'vegas', 'amp','ces2020', 'ces', '2020']
filtered = filter(lambda x: x[0].lower() not in not_topics  ,data )
ss =list(sorted(filtered, key=lambda v: v[1], reverse=True))[:100]
print(ss)
