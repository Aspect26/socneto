package cz.cuni.mff.socneto.storage.internal.api.dto;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.UUID;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class ComponentDto {
    private Long id;
    private String componentId;
    private UUID jobId;
    private ComponentType type;
    private String inputChannelName;
    private String updateChannelName;
    private ObjectNode attributes;
}
