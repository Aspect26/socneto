package cz.cuni.mff.socneto.storage.analysis.results.data.model;

import lombok.Data;
import org.springframework.data.annotation.Id;
import org.springframework.data.elasticsearch.annotations.Document;
import org.springframework.data.elasticsearch.annotations.Field;
import org.springframework.data.elasticsearch.annotations.FieldType;

import java.util.Date;
import java.util.Map;
import java.util.UUID;

@Data
@Document(indexName = "analyses")
public class SearchAnalysis {
    @Id
    @Field(type = FieldType.Keyword)
    private UUID postId;
    @Field(type = FieldType.Keyword)
    private UUID jobId;
    private Date datetime;
    @Field(type = FieldType.Keyword)
    private String componentId;
    private Map<String, SearchAnalysisResult> results;
}
