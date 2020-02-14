package cz.cuni.mff.socneto.storage.internal.model;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.Getter;

import java.util.Map;

@Data
public class RegistrationMessage {
    private String componentId;
    private String componentType;
    private String updateChannelName;
    private String inputChannelName;
    private Attributes attributes;

    @Getter
    @AllArgsConstructor
    public static class Attributes {
        private Map<String, String> outputFormat;
    }
}
