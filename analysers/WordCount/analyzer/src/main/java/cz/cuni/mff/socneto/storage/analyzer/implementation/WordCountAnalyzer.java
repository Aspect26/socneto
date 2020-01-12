package cz.cuni.mff.socneto.storage.analyzer.implementation;

import cz.cuni.mff.socneto.storage.analyzer.Analyzer;
import cz.cuni.mff.socneto.storage.model.AnalysisMessage.AnalysisResult;
import org.springframework.context.annotation.Profile;
import org.springframework.stereotype.Service;

import java.util.Arrays;
import java.util.Map;
import java.util.function.Function;
import java.util.stream.Collectors;

@Profile("WORD_COUNT")
@Service
public class WordCountAnalyzer implements Analyzer {

    @Override
    public Map<String, AnalysisResult> analyze(String text) {
        var words = text.split(" ");

        var frequency = Arrays.stream(words).collect(Collectors.groupingBy(Function.identity(),
                Collectors.collectingAndThen(Collectors.counting(), Double::valueOf)));

        var result = AnalysisResult.builder().numberMapValue(frequency).build();

        return Map.of("wordCount", result);
    }

    @Override
    public Map<String, String> getFormat() {
        return Map.of("wordCount", "numberMapValue");
    }
}
