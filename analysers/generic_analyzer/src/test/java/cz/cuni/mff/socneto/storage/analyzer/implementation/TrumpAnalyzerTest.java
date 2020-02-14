package cz.cuni.mff.socneto.storage.analyzer.implementation;


import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvFileSource;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

class TrumpAnalyzerTest {

    private static final String HASHTAGS_KEY = "trumpCount";
    private final TrumpAnalyzer trumpAnalyzer = new TrumpAnalyzer();

    @ParameterizedTest
    @CsvFileSource(resources = "trump_data.csv")
    void testHashCount(String text, int value) {
        var result = trumpAnalyzer.analyze(text);
        assertTrue(result.containsKey(HASHTAGS_KEY));
        assertEquals(value, result.get(HASHTAGS_KEY).getNumberValue());
    }

}
