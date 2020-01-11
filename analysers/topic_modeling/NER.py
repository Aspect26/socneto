import spacy
import re
#needed to install as: python -m spacy download en_core_web_lg
nlp = spacy.load("en_core_web_lg")


'''
Kategorie:
PERSON	People, including fictional.
NORP	Nationalities or religious or political groups.
FAC	Buildings, airports, highways, bridges, etc.
ORG	Companies, agencies, institutions, etc.
GPE	Countries, cities, states.
LOC	Non-GPE locations, mountain ranges, bodies of water.
PRODUCT	Objects, vehicles, foods, etc. (Not services.)
EVENT	Named hurricanes, battles, wars, sports events, etc.
WORK_OF_ART	Titles of books, songs, etc.
LAW	Named documents made into laws.
LANGUAGE	Any named language.
DATE	Absolute or relative dates or periods.
TIME	Times smaller than a day.
PERCENT	Percentage, including ”%“.
MONEY	Monetary values, including unit.
QUANTITY	Measurements, as of weight or distance.
ORDINAL	“first”, “second”, etc.
CARDINAL	Numerals that do not fall under another type.
'''
def get_named_entitites(text):

    doc = nlp(text)
    '''
    for token in doc:
        print(token.text, token.lemma_, token.pos_, token.tag_, token.dep_)
    for token in doc:
        print(token.text,"Head:", token.head, " Left:", token.left_edge, " Right:", token.right_edge, " Relationship:", token.dep_)
    '''
    refrences = re.findall( r'[@](\w+)', text)
    return [(d.text, d.label_) for d in doc.ents].append(refrences)

#a = get_named_entitites("Hillary Diane Rodham was born on October 26, 1947 in Chicago.")
#print("end")
