package cz.cuni.mff.socneto.storage.analysis.storage.api.dto;

import lombok.Builder;
import lombok.Data;


@Data
@Builder
public class PostDto {
    private String id;
    private String text;
    private String originalText;
    private String authorId;
    private String source;
    private String dateTime;
}
