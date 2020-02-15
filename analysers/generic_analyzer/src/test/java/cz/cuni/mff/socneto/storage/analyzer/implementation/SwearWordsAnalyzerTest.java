package cz.cuni.mff.socneto.storage.analyzer.implementation;

import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvFileSource;

import static org.junit.jupiter.api.Assertions.*;

class SwearWordsAnalyzerTest {

    private static final String SWEAR_KEY = "swearWords";
    private final SwearWordsAnalyzer swearWordsAnalyzer = new SwearWordsAnalyzer();

    @ParameterizedTest
    @CsvFileSource(resources = "swearWords_data.csv")
    void testHashCount(String text, int value) {
        var result = swearWordsAnalyzer.analyze(text);
        assertTrue(result.containsKey(SWEAR_KEY));
        assertNotNull(result.get(SWEAR_KEY));
        assertEquals(value, result.get(SWEAR_KEY).getNumberValue());
    }
}