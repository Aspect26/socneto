package cz.cuni.mff.socneto.storage.messaging.producer;

import org.springframework.context.annotation.Configuration;

@Configuration
public class SenderConfig {

//    @Value("${spring.messaging.bootstrap-servers}")
//    private String bootstrapServers;
//
//    @Bean
//    public Map<String, Object> producerConfigs() {
//        Map<String, Object> props = new HashMap<>();
//        props.put(ProducerConfig.BOOTSTRAP_SERVERS_CONFIG, bootstrapServers);
//        props.put(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG, StringSerializer.class);
//        props.put(ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, JsonSerializer.class);
//        return props;
//    }
//
//    @Bean
//    public ProducerFactory<String, PostDto> producerFactory() {
//        return new DefaultKafkaProducerFactory<>(producerConfigs());
//    }
//
//    @Bean
//    public KafkaTemplate<String, PostDto> kafkaTemplate() {
//        return new KafkaTemplate<>(producerFactory());
//    }

}
