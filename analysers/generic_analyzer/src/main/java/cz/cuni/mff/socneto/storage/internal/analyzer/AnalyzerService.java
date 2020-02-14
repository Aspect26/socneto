package cz.cuni.mff.socneto.storage.internal.analyzer;

import cz.cuni.mff.socneto.storage.model.AnalysisMessage;
import cz.cuni.mff.socneto.storage.model.PostMessage;

import java.util.Map;

public interface AnalyzerService {

    AnalysisMessage analyze(PostMessage postMessage);

    Map<String, String> getFormat();
}
