package cz.cuni.mff.socneto.storage.analysis.results.api.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.Date;
import java.util.UUID;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class SearchPostDto {
    private UUID id;
    private UUID jobId;
    private String originalId;
    private String text;
    private String originalText;
    private String authorId;
    private String language;
    private Date datetime;
}
