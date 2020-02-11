package cz.cuni.mff.socneto.storage.internal.api.dto;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.springframework.lang.NonNull;

import java.util.UUID;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class JobViewDto {
    @NonNull
    private UUID jobId;
    private ObjectNode viewConfiguration;
}
