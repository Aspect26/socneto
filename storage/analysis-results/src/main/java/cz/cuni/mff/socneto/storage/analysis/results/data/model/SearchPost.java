package cz.cuni.mff.socneto.storage.analysis.results.data.model;

import lombok.Data;
import org.springframework.data.annotation.Id;
import org.springframework.data.elasticsearch.annotations.Document;
import org.springframework.data.elasticsearch.annotations.Field;
import org.springframework.data.elasticsearch.annotations.FieldType;

import java.util.Date;
import java.util.UUID;

@Data
@Document(indexName = "posts", type = "post")
public class SearchPost {

    @Id
    private UUID id;
    private String originalId;
    @Field(type = FieldType.Text)
    private String text;
    private Date datetime;
}
