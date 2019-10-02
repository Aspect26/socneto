package cz.cuni.mff.socneto.storage.messaging.consumer.model;

import lombok.Data;

import java.util.UUID;

@Data
public class PostMessage {
    private UUID postId;
    private UUID jobId;
    private String text;
    private String authorId;
    private String source;
    private String dateTime;
}
