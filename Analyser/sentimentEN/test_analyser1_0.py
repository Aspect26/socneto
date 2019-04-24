import unittest
import analyser1_0 as module


class TestAnalyserMethods(unittest.TestCase):

    def test_(self):
        data = {
            1: 'fool',
            2: 'shit',
            3: 'nice'
        }
        expected = {
            1: 'neutral',
            2: 'negative',
            3: 'positive'
        }

        result = module.get_sentiments(data)
        self.assertDictEqual(result, expected, )


if __name__ == '__main__':
    unittest.main()
