package cz.cuni.mff.socneto.storage.messaging.consumer.model;

import lombok.Data;

@Data
public class RegistrationMessage {
    private String componentType;
    private String componentId;
    private String inputChannelName;
    private String updateChannelName;
}
