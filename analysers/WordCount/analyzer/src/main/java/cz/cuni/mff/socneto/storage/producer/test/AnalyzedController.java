package cz.cuni.mff.socneto.storage.producer.test;

import cz.cuni.mff.socneto.storage.analyzer.AnalyzerService;
import cz.cuni.mff.socneto.storage.model.AnalysisMessage;
import cz.cuni.mff.socneto.storage.model.PostMessage;
import cz.cuni.mff.socneto.storage.producer.AnalysisProducer;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequiredArgsConstructor
public class AnalyzedController {

    private final AnalyzerService analyzerService;
    private final AnalysisProducer analysisProducer;

    @Value("${app.config.topicDatabase")
    private String topicToDb;

    @PostMapping("internal/analysis")
    public void log(@RequestBody AnalysisMessage analysisMessage) {
        analysisProducer.send(topicToDb, analysisMessage);
    }

    @PostMapping("internal/post")
    public void log(@RequestBody PostMessage postMessage) {
        var analysisMessage = analyzerService.analyze(postMessage);
        analysisProducer.send(topicToDb, analysisMessage);
    }

}
