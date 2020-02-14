package cz.cuni.mff.socneto.storage.analyzer.implementation;

import cz.cuni.mff.socneto.storage.analyzer.Analyzer;
import cz.cuni.mff.socneto.storage.model.AnalysisMessage.AnalysisResult;
import org.springframework.context.annotation.Profile;
import org.springframework.stereotype.Service;

import java.util.Arrays;
import java.util.Map;
import java.util.function.Function;
import java.util.stream.Collectors;

/**
 * Analyzer takes text, divides it to words, transforms words to lower case
 * and creates word frequency.
 */
@Service
@Profile("WORD_COUNT")
public class WordCountAnalyzer implements Analyzer {

    @Override
    public Map<String, AnalysisResult> analyze(String text) {
        var frequency = Arrays.stream(text.split(" "))
                .map(String::toLowerCase)
                .collect(Collectors.groupingBy(Function.identity(),
                Collectors.collectingAndThen(Collectors.counting(), Double::valueOf)));

        var result = AnalysisResult.builder().numberMapValue(frequency).build();

        return Map.of("wordCount", result);
    }

    @Override
    public Map<String, String> getFormat() {
        return Map.of("wordCount", "numberMapValue");
    }
}
