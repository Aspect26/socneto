package cz.cuni.mff.socneto.storage.internal.producer;

import cz.cuni.mff.socneto.storage.model.AnalysisMessage;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.core.KafkaTemplate;
import org.springframework.stereotype.Service;

@Slf4j
@Service
@RequiredArgsConstructor
public class AnalysisProducer {

    private final KafkaTemplate<String, AnalysisMessage> analysisTemplate;

    public void send(String topic, AnalysisMessage analysisMessage){
        log.info("ACTION='analysis' TOPIC='{}' ANALYSIS='{}'", topic, analysisMessage);
        analysisTemplate.send(topic, analysisMessage);
    }
}
