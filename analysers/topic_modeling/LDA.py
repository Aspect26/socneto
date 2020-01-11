import numpy as np
#import pandas as pd
import spacy
import gensim
import gensim.corpora as corpora
import many_stop_words
#https://towardsdatascience.com/building-a-topic-modeling-pipeline-with-spacy-and-gensim-c5dc03ffc619


class LDAAnalysis:
    def __init__(self, topic_num = 5, topic_words = 10):
        self.nlp = spacy.load("en_core_web_lg")
        self.nlp.add_pipe(self.lemmatizer, name='lemmatizer', after='ner')
        self.nlp.add_pipe(self.remove_stopwords, name="stopwords", last=True)
        self.topic_num = topic_num
        self.topic_words = topic_words

    def lemmatizer(self,doc):
        # This takes in a doc of tokens from the NER and lemmatizes them.
        # Pronouns (like "I" and "you" get lemmatized to '-PRON-', so I'm removing those.
        doc = [token.lemma_ for token in doc if token.lemma_ != '-PRON-']
        doc = u' '.join(doc)
        return self.nlp.make_doc(doc)

    def remove_stopwords(self,doc):
        for word in many_stop_words.get_stop_words("en"):
            lexeme = self.nlp.vocab[word]
            lexeme.is_stop = True
        doc = [token.text for token in doc if token.is_stop != True and token.is_punct != True]
        return doc

    def get_topic_keywords(self,text):
        pr = [self.nlp(text)]
        words = corpora.Dictionary(pr)
        corpus = [words.doc2bow(d) for d in pr]
        lda_model = gensim.models.ldamodel.LdaModel(corpus=corpus,
                                                    id2word=words,
                                                    num_topics=5,
                                                    random_state=2,
                                                    update_every=1,
                                                    passes=10,
                                                    alpha='auto',
                                                    per_word_topics=True)
        return lda_model.show_topics(self.topic_num,self.topic_words)


lda = LDAAnalysis()
a = lda.get_topic_keywords("Python, like most many programming languages, has a huge amount of exceptional libraries and modules to choose from. Generally of course, this is absolutely brilliant, but it also means that sometimes the modules don’t always play nicely with each other. In this short tutorial, I’m going to show you how to link up spaCy with Gensim to create a coherent topic modeling pipeline.")

for (topic, tt) in a:
    print (topic)
    print(tt)
