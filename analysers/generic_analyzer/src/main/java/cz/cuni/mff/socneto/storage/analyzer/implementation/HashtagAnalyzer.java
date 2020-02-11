package cz.cuni.mff.socneto.storage.analyzer.implementation;

import cz.cuni.mff.socneto.storage.analyzer.Analyzer;
import cz.cuni.mff.socneto.storage.model.AnalysisMessage.AnalysisResult;
import org.springframework.context.annotation.Profile;
import org.springframework.stereotype.Service;

import java.util.Arrays;
import java.util.Map;
import java.util.stream.Collectors;

/**
 * Analyzer takes text, divides it to words, transforms words to lower case
 * and counts distinct words, which start with #.
 */
@Service
@Profile("HASH_TAG")
public class HashtagAnalyzer implements Analyzer {

    @Override
    public Map<String, AnalysisResult> analyze(String text) {
        var hashtags = Arrays.stream(text.split(" "))
                .map(String::toLowerCase)
                .distinct()
                .filter(word -> word.startsWith("#"))
                .map(word -> word.substring(1))
                .filter(s -> !s.isBlank())
                .collect(Collectors.toList());

        var result = AnalysisResult.builder().textListValue(hashtags).build();

        return Map.of("hashtags", result);
    }

    @Override
    public Map<String, String> getFormat() {
        return Map.of("hashtags", "textListValue");
    }
}
