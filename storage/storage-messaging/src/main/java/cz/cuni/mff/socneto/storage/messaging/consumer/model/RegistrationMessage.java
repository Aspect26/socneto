package cz.cuni.mff.socneto.storage.messaging.consumer.model;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.Data;

@Data
public class RegistrationMessage {
    private String componentType;
    private String componentId;
    private String inputChannelName;
    private String updateChannelName;
    private ObjectNode attributes;
}
