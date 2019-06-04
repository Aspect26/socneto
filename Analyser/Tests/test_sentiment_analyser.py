import unittest
import Analyser.sentiment_analysis.sentiment_analyser as sa


class TestAnalyserMethods(unittest.TestCase):

    def test_(self):
                data = {
                    1: 'fool',
                    2: 'shit',
                    3: 'nice'
                }
                expected = {
                    1: 2,
                    2: 0,
                    3: 1
                }

                result = sa.get_sentiments(data)
                self.assertDictEqual(result, expected, )


if __name__ == '__main__':
    unittest.main()
