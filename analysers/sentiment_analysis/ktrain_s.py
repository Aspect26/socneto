# -*- coding: utf-8 -*-
"""Copy of public-transformers_in_ktrain.ipynb

Automatically generated by Colaboratory.

Original file is located at
    https://colab.research.google.com/drive/1kOoyTnB53mXtL3_9-DkstKMhGUuBTkWz

### A Simplied Interface to Text Classification With Hugging Face Transformers in TensorFlow Using [ktrain](https://github.com/amaiya/ktrain)

*ktrain* requires TensorFlow 2.
"""

#!pip3 install -q tensorflow_gpu

import os
os.getcwd()

import tensorflow as tf
print(tf.__version__)
from sklearn.model_selection import train_test_split

"""We then need to install *ktrain* library using pip."""

#!pip3 install -q ktrain

"""### Load a Dataset Into Arrays"""

import pandas as pd
import ktrain
from ktrain import text

train = pd.read_csv('/Users/petravysusilova/Documents/school/SWProject/ktrain_reseni/airlines_data.csv', sep='\t', header=None)
#categories = train[1].unique().tolist()
categories = ['neutral','positive','negative']
print(train.head())

train = train.head(10)

split_ratio = 0.5

#random shuffle
train, validate = train_test_split(train, test_size=split_ratio, shuffle = False)

print('size of training set: %s' % (len(train)))
print('size of validation set: %s' % (len(validate)))


x_train = train.iloc[:,0]
y_train = train.iloc[:,1]
x_test = validate.iloc[:,0]
y_test = validate.iloc[:,1]

"""## STEP 1:  Preprocess Data and Create a Transformer Model

We will use [DistilBERT](https://arxiv.org/abs/1910.01108).
"""



MODEL_NAME = 'distilbert-base-uncased'
t = text.Transformer(MODEL_NAME, maxlen=500, classes=categories)
trn = t.preprocess_train(x_train, y_train)
val = t.preprocess_test(x_test, y_test)
model = t.get_classifier()
learner = ktrain.get_learner(model, train_data=trn, val_data=val, batch_size=50)

"""## STEP 2:  Train the Model"""

learner.fit_onecycle(5e-5, 1)

"""## STEP 3: Evaluate and Inspect the Model"""

learner.validate()

"""Let's examine the validation example about which we were the most wrong."""

#learner.view_top_losses(n=1, preproc=t)

#print(x_test[371])

"""This post talks more about computing than `alt.atheism` (the true category), so our model placed it into the only computing category available to it: `comp.graphics`

## STEP 4: Making Predictions on New Data in Deployment
"""

predictor = ktrain.get_predictor(learner.model, preproc=t)

#predictor.predict('Fuck you.')

# predicted probability scores for each category
#predictor.predict_proba('Jesus Christ is the central figure of Christianity.')

#predictor.get_classes()

"""As expected, `soc.religion.christian` is assigned the highest probability.

Let's invoke the `explain` method to see which words contribute most to the classification.

We will need a forked version of the **eli5** library that supportes TensorFlow Keras, so let's install it first.
"""

#!pip3 install -q git+https://github.com/amaiya/eli5@tfkeras_0_10_1

#predictor.explain('Jesus Christ is the central figure in Christianity.')

"""The words in the darkest shade of green contribute most to the classification and agree with what you would expect for this example.

We can save and reload our predictor for later deployment.
"""

predictor.save('./distilbert_predictor')

reloaded_predictor = ktrain.load_predictor('./distilbert_predictor')

print(reloaded_predictor.predict('My computer monitor is really blurry.'))

