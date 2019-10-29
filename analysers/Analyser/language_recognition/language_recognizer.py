from langdetect import detect_langs

mapping = {
    'other': 0,
    'cs': 1,
    'en': 2
}

def _get_lang_prob(language, list):
    # returns probability for given language
    result = [x.prob for x in list if x.lang == language]
    return result[0] if len(result) > 0 else 0


def _decide_right_language(result):
    # returns 0 for other, 1 for czech, 2 for english
    # decides which classification is correct.

    probabs = {}

    probabs['cs'] = _get_lang_prob('cs', result)
    probabs['en'] = _get_lang_prob('en', result)
    probabs['other'] = 0 if (probabs['cs'] + probabs['en']) > 0 else 1

    return mapping[max(probabs, key=probabs.get)]


def detect_language(text):
    """
    Function for recognize language
    :param text: text for recognition
    :return: 0 for other, 1 for czech,2 for english
    """
    try:
        res = detect_langs(text.decode("latin-1"))
        return _decide_right_language(res)
    except:
        return 0