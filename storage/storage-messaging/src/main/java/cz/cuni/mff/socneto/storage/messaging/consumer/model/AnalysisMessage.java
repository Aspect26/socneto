package cz.cuni.mff.socneto.storage.messaging.consumer.model;

import com.fasterxml.jackson.databind.JsonNode;
import lombok.Data;

import java.util.UUID;

@Data
public class AnalysisMessage {
    private UUID postId;
    private UUID jobId;
    private String componentId;

    private JsonNode analysis;
}
