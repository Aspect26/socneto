package cz.cuni.mff.socneto.storage.model;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.Data;

import java.util.Date;

@Data
public class LogMessage {

    private String componentId;
    private String eventType; // FATAL, ERROR, WARN, INFO, METRIC
    private String eventName;
    private String message;
    private Date timestamp;
    private ObjectNode attributes;

}
