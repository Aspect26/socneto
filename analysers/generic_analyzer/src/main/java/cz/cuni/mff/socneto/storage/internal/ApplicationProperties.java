package cz.cuni.mff.socneto.storage.internal;

import lombok.Getter;
import lombok.Setter;
import org.springframework.boot.context.properties.ConfigurationProperties;

@Setter
@Getter
@ConfigurationProperties("app.config")
public class ApplicationProperties {
    private String topicRegistration;
    private String topicDatabase;
    private String topicLogging;
}
