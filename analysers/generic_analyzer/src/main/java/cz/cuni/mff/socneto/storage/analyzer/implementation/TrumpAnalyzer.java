package cz.cuni.mff.socneto.storage.analyzer.implementation;

import cz.cuni.mff.socneto.storage.analyzer.Analyzer;
import cz.cuni.mff.socneto.storage.model.AnalysisMessage.AnalysisResult;
import org.springframework.context.annotation.Profile;
import org.springframework.stereotype.Service;

import java.util.Arrays;
import java.util.Map;

/**
 * Analyzer takes text, divides it to words
 * and counts distinct words, which equals to Trump ignoring case.
 */
@Service
@Profile("TRUMP_ANALYZER")
public class TrumpAnalyzer implements Analyzer {

    @Override
    public Map<String, AnalysisResult> analyze(String text) {
        var words = text.split(" ");

        var trumps = Arrays.stream(words)
                .filter(word -> word.equalsIgnoreCase("Trump"))
                .count();

        var result = AnalysisResult.builder().numberValue((double) trumps).build();

        return Map.of("trumpCount", result);
    }

    @Override
    public Map<String, String> getFormat() {
        return Map.of("trumpCount", "numberValue");
    }
}
