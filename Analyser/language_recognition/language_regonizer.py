from langdetect import detect, detect_langs

data = ['ahoj. jak se vede?', 'sure. it will be ok.', 'strom roste v lese.']
res = detect("War doesn't show who's right, just who's left.")

for i in data:
    res = detect_langs(i)
    print(res)

#if cz/en in first 3 languages