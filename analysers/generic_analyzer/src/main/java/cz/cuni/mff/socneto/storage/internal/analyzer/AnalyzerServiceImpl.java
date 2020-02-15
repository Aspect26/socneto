package cz.cuni.mff.socneto.storage.internal.analyzer;

import cz.cuni.mff.socneto.storage.analyzer.Analyzer;
import cz.cuni.mff.socneto.storage.internal.ComponentProperties;
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

        return AnalysisMessage.builder()
                .componentId(componentProperties.getComponentId())
                .jobId(postMessage.getJobId())
                .postId(postMessage.getPostId())
                .results(results)
                .build();
    }

    @Override
    public Map<String, String> getFormat() {
        return analyzer.getFormat();
    }
}
