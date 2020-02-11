package cz.cuni.mff.socneto.storage.analyzer.implementation;


import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvFileSource;

import java.util.stream.Collectors;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class WordCountAalyzerTest {

    private static final String HASHTAGS_KEY = "wordCount";
    private final WordCountAnalyzer wordCountAnalyzer = new WordCountAnalyzer();

    @ParameterizedTest
    @CsvFileSource(resources = "wordCount_data.csv")
    void testHashCount(String text, int groupCount, int wordCount) {
        var result = wordCountAnalyzer.analyze(text);
        assertTrue(result.containsKey(HASHTAGS_KEY));
        assertEquals(groupCount, result.get(HASHTAGS_KEY).getNumberMapValue().keySet().size());
        assertEquals(wordCount, result.get(HASHTAGS_KEY).getNumberMapValue().values().stream()
                .collect(Collectors.summarizingDouble(g -> g)).getSum());
    }

}
