package cz.cuni.mff.socneto.storage.model;

import lombok.Builder;
import lombok.Data;

import java.util.Map;

@Data
@Builder
public class RegistrationMessage {
    private String componentId;
    private String componentType;
    private String updateChannelName;
    private String inputChannelName;
    private Map<String, String> resultsFormat;
}
