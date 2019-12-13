package cz.cuni.mff.socneto.storage.producer;

import cz.cuni.mff.socneto.storage.model.AnalysisMessage;
import cz.cuni.mff.socneto.storage.model.LogMessage;
import cz.cuni.mff.socneto.storage.model.RegistrationMessage;
import org.apache.kafka.clients.producer.ProducerConfig;
import org.apache.kafka.common.serialization.StringSerializer;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.kafka.core.DefaultKafkaProducerFactory;
import org.springframework.kafka.core.KafkaTemplate;
import org.springframework.kafka.core.ProducerFactory;
import org.springframework.kafka.support.serializer.JsonSerializer;

import java.util.HashMap;
import java.util.Map;

@Configuration
public class ProducerConfiguration {

    @Value("${spring.messaging.bootstrap-servers}")
    private String bootstrapServers;

    @Bean
    public Map<String, Object> producerConfigs() {
        Map<String, Object> props = new HashMap<>();
        props.put(ProducerConfig.BOOTSTRAP_SERVERS_CONFIG, bootstrapServers);
        props.put(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG, StringSerializer.class);
        props.put(ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, JsonSerializer.class);
        return props;
    }

    @Bean
    public ProducerFactory<String, LogMessage> logProducerFactory() {
        return new DefaultKafkaProducerFactory<>(producerConfigs());
    }

    @Bean
    public KafkaTemplate<String, LogMessage> logKafkaTemplate() {
        return new KafkaTemplate<>(logProducerFactory());
    }

    @Bean
    public ProducerFactory<String, AnalysisMessage> analysisProducerFactory() {
        return new DefaultKafkaProducerFactory<>(producerConfigs());
    }

    @Bean
    public KafkaTemplate<String, AnalysisMessage> analysisKafkaTemplate() {
        return new KafkaTemplate<>(analysisProducerFactory());
    }

    @Bean
    public ProducerFactory<String, RegistrationMessage> registrationProducerFactory() {
        return new DefaultKafkaProducerFactory<>(producerConfigs());
    }

    @Bean
    public KafkaTemplate<String, RegistrationMessage> registrationKafkaTemplate() {
        return new KafkaTemplate<>(registrationProducerFactory());
    }

}
