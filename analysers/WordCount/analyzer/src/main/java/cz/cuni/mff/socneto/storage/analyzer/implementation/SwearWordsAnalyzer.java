package cz.cuni.mff.socneto.storage.analyzer.implementation;

import cz.cuni.mff.socneto.storage.analyzer.Analyzer;
import cz.cuni.mff.socneto.storage.model.AnalysisMessage.AnalysisResult;
import org.springframework.context.annotation.Profile;
import org.springframework.stereotype.Service;

import java.util.Arrays;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

@Profile("SWEAR_WORDS")
@Service
public class SwearWordsAnalyzer implements Analyzer {

    private final Set<String> swearWords = new HashSet<>(
            Arrays.asList("arse", "ass", "asshole", "bastard", "bitch", "bollocks", "bugger",
                    "child-fucker", "crap", "cunt", "damn", "effing", "frigger", "fuck", "goddamn",
                    "godsdamn", "hell", "holy shit", "horseshit", "motherfucker", "nigga", "nigger",
                    "prick", "shit", "shitass", "slut", "twat"
            )
    );

    @Override
    public Map<String, AnalysisResult> analyze(String text) {
        var words = text.split(" ");

        var trumps = Arrays.stream(words)
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
