package cz.cuni.mff.socneto.storage;

import cz.cuni.mff.socneto.storage.messaging.consumer.KafkaConsumerConfig;
import cz.cuni.mff.socneto.storage.analysis.AnalysisStorageConfiguration;
import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Import;

@Import({KafkaConsumerConfig.class, AnalysisStorageConfiguration.class, StorageWebConfiguration.class})
@SpringBootApplication
public class StorageWebApplication implements ApplicationRunner {

    public static void main(String[] args) {
        SpringApplication.run(StorageWebApplication.class, args);
    }

    @Override
    public void run(ApplicationArguments args) throws Exception {

    }
}