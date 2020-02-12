package cz.cuni.mff.socneto.storage.internal.api.dto;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class ComponentDto {
    @NotBlank
    private String componentId;
    @NotNull
    private ComponentType type;
    @NotBlank
    private String inputChannelName;
    @NotBlank
    private String updateChannelName;
    private ObjectNode attributes;
}
