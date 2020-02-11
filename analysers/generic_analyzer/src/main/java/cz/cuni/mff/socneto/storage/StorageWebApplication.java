package cz.cuni.mff.socneto.storage;

import cz.cuni.mff.socneto.storage.internal.ApplicationProperties;
import cz.cuni.mff.socneto.storage.internal.ComponentProperties;
import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.annotation.Configuration;


@Configuration
@SpringBootApplication
@EnableConfigurationProperties({ComponentProperties.class, ApplicationProperties.class})
public class StorageWebApplication implements ApplicationRunner {

    public static void main(String[] args) {
        SpringApplication.run(StorageWebApplication.class, args);
    }

    @Override
    public void run(ApplicationArguments args) throws Exception {

    }
}