package cz.cuni.mff.socneto.storage.analysis.results.api.dto;

import lombok.Data;

import java.util.Date;
import java.util.Map;
import java.util.UUID;

@Data
public class SearchAnalysisDto {
    private UUID jobId;
    private UUID postId;
    private Date datetime;
    private String componentId;
    private Map<String, SearchAnalysisResultDto> results;
}
