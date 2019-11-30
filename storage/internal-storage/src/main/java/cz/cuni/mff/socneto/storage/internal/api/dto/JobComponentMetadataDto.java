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
public class JobComponentMetadataDto {
    private UUID id;
    private String componentId;
    private UUID jobId;
    private boolean finished;
    private ObjectNode componentAttributes;
    private ObjectNode jobAttributes;
    private ObjectNode componentMetadata;
}
