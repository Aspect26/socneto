package cz.cuni.mff.socneto.storage.analyzer;

import cz.cuni.mff.socneto.storage.ComponentProperties;
import cz.cuni.mff.socneto.storage.model.AnalysisMessage;
import cz.cuni.mff.socneto.storage.model.PostMessage;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.Map;

@Service
@RequiredArgsConstructor
public class AnalyzerServiceImpl implements AnalyzerService {

    private final ComponentProperties componentProperties;
    private final Analyzer analyzer;

    @Override
    public AnalysisMessage analyze(PostMessage postMessage) {
        var results = analyzer.analyze(postMessage.getText());

        var analysisMessage = new AnalysisMessage();
        analysisMessage.setComponentId(componentProperties.getComponentId());
        analysisMessage.setJobId(postMessage.getJobId());
        analysisMessage.setPostId(postMessage.getPostId());
        analysisMessage.setResults(results);
        return analysisMessage;
    }

    @Override
    public Map<String, String> getFormat() {
        return analyzer.getFormat();
    }
}
