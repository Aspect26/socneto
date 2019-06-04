import Analyser.language_recognition.language_recognizer as lr
import unittest

class testLanguageRecogintion(unittest.TestCase):
    def test_(self):
        data = ['Ahoj. Tpická česká věta. Kde domuv můj?', 'Winter is coming. I am no one.', 'Hello világ. Hogy vagy?']
        expected = [1, 2, 0]

        for text, res in zip(data, expected):
            result = lr.detect_language(text)
            self.assertEqual(result, res)



if __name__ == '__main__':
    unittest.main()