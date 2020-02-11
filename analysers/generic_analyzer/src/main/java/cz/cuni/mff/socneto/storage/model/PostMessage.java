package cz.cuni.mff.socneto.storage.model;

import lombok.Builder;
import lombok.Data;

import java.util.Date;
import java.util.UUID;

/**
 * Input post message, see Analyzer documentation.
 */
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
