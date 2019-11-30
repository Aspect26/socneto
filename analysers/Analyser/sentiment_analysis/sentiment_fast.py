# using FastText library
import fasttext
import pandas as pd
from sklearn import model_selection as ms
from datetime import datetime
from sklearn.metrics import accuracy_score

# prints evaluation of model
# source: https://github.com/facebookresearch/fastText/tree/master/python#installation
def print_results(N, p, r):
    print("N\t" + str(N))
    print("P@{}\t{:.3f}".format(1, p))
    print("R@{}\t{:.3f}".format(1, r))


def create_dataset():
    data_czech = pd.read_csv(
        "/Users/petravysusilova/Documents/School/SWProject/SWProject/data/Datasets/facebook/gold-posts.txt",
        delimiter='\n', names=[5])
    labels_czech = pd.read_csv(
        "/Users/petravysusilova/Documents/School/SWProject/SWProject/data/Datasets/facebook/gold-labels.txt",
        delimiter='\n', names=[5])
    data_en = pd.read_csv(
        "/Users/petravysusilova/Documents/School/SWProject/SWProject/data/Datasets/trainingandtestdata/training.1600000.processed.noemoticon.csv",
        delimiter=',', header=None, usecols=[0, 5], encoding='latin1')

    labels_czech = labels_czech.replace(to_replace={'p': '__label__positive', 'n': '__label__negative', '0': '__label__neutral'})
    data_czech['sentiment'] = labels_czech
    data_czech = data_czech[data_czech['sentiment'] != 'b']
    data_en.rename(columns={0:'sentiment'}, inplace=True)
    data_en.replace(to_replace={4:'__label__positive', 0 : '__label__negative', 2:'__label__neutral'}, inplace=True)

    data_czech['lang'] = 1
    data_en['lang'] = 2

    merged = pd.concat([data_czech, data_en], sort=True)
    merged = merged.sample(frac=1, random_state=42)
    merged.rename(columns={5 :'text'}, inplace=True)

    merged = merged[['sentiment','text']]
    return merged

def create_model(file):
    # TODO
    # vytvorit dataset vypdajici _label_slovolabel post pro czech a english dohromady a rozdelit na train a test
    dataset = create_dataset()
    train_data, test_data = ms.train_test_split(
        dataset, test_size=0.1, random_state=42)

    train_data.to_csv('train_data.txt', encoding='utf-8', index=False, sep=' ', header=False)
    test_data[['text']].to_csv('test_data.txt', encoding='utf-8', index=False, header=False)
    test_data[['sentiment']].to_csv('test_labels.txt', encoding='utf-8', index=False, header=False)
    test_data.to_csv('test_data_all', encoding='utf-8', index=False, header=False, sep=' ')
    # natrenovat model na train
    model = fasttext.train_supervised('train_data.txt')

    # current date and time

    model.save_model(file)


now = datetime.now()
timestamp = datetime.timestamp(now)
file = "model_" + str(timestamp) + ".bin"
create_model(file)

#TESTOVANI ACCURACY (priklad pouziti)
model = fasttext.load_model(file)
print_results(*model.test('test_data_all'))

test = pd.read_csv('test_data.txt', encoding='utf-8')
test = [item for sublist in test.values.tolist() for item in sublist]
test_labels = pd.read_csv('test_labels.txt')
pred = model.predict(test)
pred = [item for sublist in pred[0] for item in sublist]
lab = [item for sublist in test_labels.values.tolist() for item in sublist]
print(accuracy_score(lab, pred))

