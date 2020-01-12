package cz.cuni.mff.socneto.storage.analyzer;

import cz.cuni.mff.socneto.storage.model.AnalysisMessage.AnalysisResult;

import java.util.Map;

public interface Analyzer {

    Map<String, AnalysisResult> analyze(String text);

    Map<String, String> getFormat();
}
