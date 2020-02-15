package cz.cuni.mff.socneto.storage.internal.model;

import lombok.Data;

import java.util.Map;

@Data
public class RegistrationMessage {
    private String componentId;
    private String componentType;
    private String updateChannelName;
    private String inputChannelName;
    private Map<String, String> resultsFormat;
}
