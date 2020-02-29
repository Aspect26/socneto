package cz.cuni.mff.socneto.storage.analysis.results.data.model;

import lombok.Data;
import org.springframework.data.annotation.Id;
import org.springframework.data.elasticsearch.annotations.Document;
import org.springframework.data.elasticsearch.annotations.Field;
import org.springframework.data.elasticsearch.annotations.FieldType;
import org.springframework.data.elasticsearch.annotations.Setting;

import java.util.Date;
import java.util.UUID;

@Data
@Document(indexName = "posts")
@Setting(settingPath = "/setting.json")
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
    @Field(type = FieldType.Keyword)
    private String authorId;
    @Field(type = FieldType.Keyword)
    private String language;
    @Field(type = FieldType.Date)
    private Date datetime;
}
