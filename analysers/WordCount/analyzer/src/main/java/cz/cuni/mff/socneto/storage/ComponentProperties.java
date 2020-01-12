package cz.cuni.mff.socneto.storage;

import lombok.Getter;
import lombok.Setter;
import org.springframework.boot.context.properties.ConfigurationProperties;

@Setter
@Getter
@ConfigurationProperties("component.config")
public class ComponentProperties {
    private String componentId;
    private String componentType;
    private String topicUpdate;
    private String topicInput;
}
