package cz.cuni.mff.socneto.storage.internal.producer;

import cz.cuni.mff.socneto.storage.model.LogMessage;
import cz.cuni.mff.socneto.storage.internal.model.RegistrationMessage;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.core.KafkaTemplate;
import org.springframework.stereotype.Service;

@Slf4j
@Service
@RequiredArgsConstructor
public class RegistrationProducer {

    private final LogProducer logProducer;
    private final KafkaTemplate<String, RegistrationMessage> analysisTemplate;

    public void send(String topic, RegistrationMessage registrationMessage) {
        log.info("ACTION='registration' TOPIC='{}' ANALYSIS='{}'", topic, registrationMessage);

        analysisTemplate.send(topic, registrationMessage);

        logProducer
                .send(
                        LogMessage.builder()
                                .eventType(LogMessage.EventType.INFO)
                                .eventName("ComponentRegistered")
                                .message("ComponentRegistered")
                                .build()
                );
    }
}
