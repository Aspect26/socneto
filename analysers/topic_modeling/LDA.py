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

    def format(self, model, words):
        all_words = []
        all_probabs = []
        for i in range(self.topic_num):
            indexy, pravdepod = map(list, zip(* model.get_topic_terms(i)))
            slova = [words[ind] for ind in indexy]
            all_words.append(slova)
            all_probabs.append(pravdepod)
        return all_words, all_probabs


    def get_topic_keywords(self,text):
        pr = [self.nlp(text)]
        words = corpora.Dictionary(pr)
        corpus = [words.doc2bow(d) for d in pr]
        lda_model = gensim.models.ldamodel.LdaModel(corpus=corpus,
                                                    id2word=words,
                                                    num_topics=self.topic_num,
                                                    random_state=2,
                                                    update_every=1,
                                                    passes=10,
                                                    alpha='auto',
                                                    per_word_topics=True)
        return self.format(lda_model, words)

