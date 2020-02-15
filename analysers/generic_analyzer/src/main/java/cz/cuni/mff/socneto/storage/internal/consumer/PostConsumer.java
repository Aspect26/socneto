package cz.cuni.mff.socneto.storage.internal.consumer;

import com.fasterxml.jackson.databind.ObjectMapper;
import cz.cuni.mff.socneto.storage.internal.ApplicationProperties;
import cz.cuni.mff.socneto.storage.internal.analyzer.AnalyzerService;
import cz.cuni.mff.socneto.storage.model.PostMessage;
import cz.cuni.mff.socneto.storage.internal.producer.AnalysisProducer;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Service;

import java.io.IOException;

@Slf4j
@Service
@RequiredArgsConstructor
public class PostConsumer {

    private final ApplicationProperties applicationProperties;

    private final AnalyzerService analyzerService;
    private final AnalysisProducer analysisProducer;

    private final ObjectMapper objectMapper = new ObjectMapper();

    @KafkaListener(topics = "${component.config.topicInput}")
    public void listenAnalyzedPosts(@Payload String post) throws IOException {
        var postMessage = objectMapper.readValue(post, PostMessage.class);
        var analysisMessage = analyzerService.analyze(postMessage);
        analysisProducer.send(applicationProperties.getTopicDatabase(), analysisMessage);
    }
}
