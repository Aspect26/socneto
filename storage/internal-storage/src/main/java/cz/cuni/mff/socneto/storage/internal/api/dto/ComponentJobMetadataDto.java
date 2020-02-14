package cz.cuni.mff.socneto.storage.internal.api.dto;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import java.util.UUID;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class ComponentJobMetadataDto {
    private Long id;
    @NotBlank
    private String componentId;
    @NotNull
    private UUID jobId;
    private ObjectNode componentMetadata;
}
