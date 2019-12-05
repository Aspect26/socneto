package cz.cuni.mff.socneto.storage.messaging.consumer.model;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.Data;

import java.util.List;
import java.util.UUID;

@Data
public class NewJobMessage {
    private UUID jobId;
    private List<String> outputChannelNames;
    private ObjectNode attributes;
}
