package cz.cuni.mff.socneto.storage.analysis.results.data.model;

import lombok.Data;
import org.springframework.data.annotation.Id;
import org.springframework.data.elasticsearch.annotations.Document;

import java.util.Map;
import java.util.UUID;

@Data
@Document(indexName = "analyses", type = "analysis")
public class SearchAnalysis {
    @Id
    private String id;
    private UUID postId;
    private UUID jobId;
    private String componentId;
    private Map<String, SearchAnalysisResult> results;
}
