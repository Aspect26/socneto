package cz.cuni.mff.socneto.storage.analyzer.implementation;

import cz.cuni.mff.socneto.storage.analyzer.Analyzer;
import cz.cuni.mff.socneto.storage.model.AnalysisMessage.AnalysisResult;
import org.springframework.context.annotation.Profile;
import org.springframework.stereotype.Service;

import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Arrays;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

/**
 * Analyzer takes text, divides it to words, transforms words to lower case
 * and counts distinct swear words. The list with swear words is stored in data file.
 */
@Service
@Profile("SWEAR_WORDS")
public class SwearWordsAnalyzer implements Analyzer {

    private final Set<String> swearWords = getResourceData();

    private HashSet<String> getResourceData() {
        try {
            return new HashSet<>(Files.readAllLines(Path.of(getClass().getResource("swear_words.txt").toURI())));
        } catch (Exception e) {
            throw new RuntimeException("Can't read resrouce file.");
        }
    }


    @Override
    public Map<String, AnalysisResult> analyze(String text) {
        var trumps = Arrays.stream(text.split(" "))
                .filter(word -> swearWords.contains(word.toLowerCase()))
                .count();

        var result = AnalysisResult.builder().numberValue((double) trumps).build();

        return Map.of("swearWords", result);
    }

    @Override
    public Map<String, String> getFormat() {
        return Map.of("swearWords", "numberValue");
    }
}
