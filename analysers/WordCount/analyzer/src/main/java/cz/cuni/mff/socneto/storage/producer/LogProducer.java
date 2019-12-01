package cz.cuni.mff.socneto.storage.producer;

import cz.cuni.mff.socneto.storage.ApplicationProperties;
import cz.cuni.mff.socneto.storage.model.LogMessage;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.core.KafkaTemplate;
import org.springframework.stereotype.Service;

@Slf4j
@Service
@RequiredArgsConstructor
public class LogProducer {

    private final ApplicationProperties applicationProperties;
    private final KafkaTemplate<String, LogMessage> logTemplate;

    public void send(LogMessage logMessage) {
        log.info("ACTION='log' MESSAGE='{}'", logMessage);
        logTemplate.send(applicationProperties.getTopicLogging(), logMessage);
    }
}
