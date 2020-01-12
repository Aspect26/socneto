package cz.cuni.mff.socneto.storage.model;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.Builder;
import lombok.Data;

import java.util.Date;

@Data
@Builder
public class LogMessage {

    private String componentId;
    private EventType eventType;
    private String eventName;
    private String message;
    private Date timestamp;
    private ObjectNode attributes;

    public enum EventType {
        FATAL, ERROR, WARN, INFO, METRIC;
    }
}
