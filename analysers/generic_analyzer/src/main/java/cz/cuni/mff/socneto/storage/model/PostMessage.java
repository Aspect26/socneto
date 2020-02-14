package cz.cuni.mff.socneto.storage.model;

import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.Date;
import java.util.UUID;

/**
 * Input post message, see Analyzer documentation.
 */
@Data
@NoArgsConstructor
public class PostMessage {
    private UUID postId;
    private String originalPostId;
    private UUID jobId;
    private String text;
    private String originalText;
    private String source;
    private String authorId;
    private String query;
    private Date dateTime;
}
