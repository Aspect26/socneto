package cz.cuni.mff.socneto.storage.messaging.consumer.model;

import lombok.Data;

import java.util.Date;
import java.util.UUID;

@Data
public class PostMessage {
    private UUID postId;
    private String originalPostId;
    private UUID jobId;
    private String text;
    private String originalText;
    private String source;
    private String authorId;
    private String language;
    private String query;
    private Date dateTime;
}
