package cz.cuni.mff.socneto.storage.analysis.data.model;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class Post {
    private String id;
    private String text;
    private String authorId;
}
