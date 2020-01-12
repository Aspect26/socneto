package cz.cuni.mff.socneto.storage.internal.api.dto;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class ComponentDto {
    private String componentId;
    private ComponentType type;
    private String inputChannelName;
    private String updateChannelName;
    private ObjectNode attributes;
}
