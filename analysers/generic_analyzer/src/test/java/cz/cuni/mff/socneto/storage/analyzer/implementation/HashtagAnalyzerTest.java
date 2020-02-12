package cz.cuni.mff.socneto.storage.analyzer.implementation;


import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvFileSource;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

class HashtagAnalyzerTest {

    private static final String HASHTAGS_KEY = "hashtags";
    private final HashtagAnalyzer hashtagAnalyzer = new HashtagAnalyzer();

    @ParameterizedTest
    @CsvFileSource(resources = "hashtags_data.csv")
    void testHashCount(String text, int value) {
        var result = hashtagAnalyzer.analyze(text);
        assertTrue(result.containsKey(HASHTAGS_KEY));
        assertNotNull(result.get(HASHTAGS_KEY));
        assertEquals(value, result.get(HASHTAGS_KEY).getTextListValue().size());
    }

}
