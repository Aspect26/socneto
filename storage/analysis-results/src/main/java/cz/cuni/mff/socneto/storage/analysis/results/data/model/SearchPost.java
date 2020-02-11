package cz.cuni.mff.socneto.storage.analysis.results.data.model;

import lombok.Data;
import org.springframework.data.annotation.Id;
import org.springframework.data.elasticsearch.annotations.Document;
import org.springframework.data.elasticsearch.annotations.Field;
import org.springframework.data.elasticsearch.annotations.FieldType;

import java.util.Date;
import java.util.UUID;

@Data
@Document(indexName = "posts")
public class SearchPost {

    @Id
    @Field(type = FieldType.Keyword)
    private UUID id;
    @Field(type = FieldType.Keyword)
    private UUID jobId;
    @Field(type = FieldType.Keyword)
    private String originalId;
    @Field(type = FieldType.Text)
    private String text;
    @Field(type = FieldType.Text)
    private String originalText;
    private Date datetime;
}
