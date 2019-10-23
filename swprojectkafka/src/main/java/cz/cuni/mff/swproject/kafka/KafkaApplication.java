package cz.cuni.mff.swproject.kafka;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.kafka.core.KafkaTemplate;

@SpringBootApplication
public class KafkaApplication implements ApplicationRunner {
    public static final String TOPIC = "test-consumer-group";
    @Autowired
    private KafkaTemplate<String, String> kafkaTemplate;

    public static void main(String[] args) {
        SpringApplication.run(KafkaApplication.class, args);
    }

    public void sendMessage(String msg) {
        kafkaTemplate.send(TOPIC, msg);
    }

    @KafkaListener(id = "fooGroup", topics = TOPIC)
    public void listen(String message) {
        System.out.println("Received Messasge in group - group-id: " + message);
    }

    @Override
    public void run(ApplicationArguments args) throws Exception {
        sendMessage("Hi Welcome to Spring For Apache Kafka");

        while (true) {
            Thread.sleep(1000);
            System.out.println("send");
            sendMessage("Hi Welcome to Spring For Apache Kafka");

        }
    }
}