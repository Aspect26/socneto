package cz.cuni.mff.socneto.storage.model;

import lombok.Builder;
import lombok.Data;

import java.util.Date;
import java.util.UUID;

@Data
@Builder
public class PostMessage {
    private UUID postId;
    private String originalPostId;
    private UUID jobId;
    private String text;
    private String source;
    private String authorId;
    private Date dateTime;
}
