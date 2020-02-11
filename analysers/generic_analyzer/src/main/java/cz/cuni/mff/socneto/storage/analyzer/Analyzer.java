package cz.cuni.mff.socneto.storage.analyzer;

import cz.cuni.mff.socneto.storage.model.AnalysisMessage.AnalysisResult;

import java.util.Map;

/**
 * Extension for analyser implementations
 *
 * Each analyzer must return at least one AnalysisResult.
 * For description see analyzer documentation (AnalyzedPost and Format for registration).
 */
public interface Analyzer {

    Map<String, AnalysisResult> analyze(String text);

    Map<String, String> getFormat();
}
