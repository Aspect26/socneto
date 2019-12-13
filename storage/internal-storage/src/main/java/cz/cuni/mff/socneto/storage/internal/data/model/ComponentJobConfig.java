package cz.cuni.mff.socneto.storage.internal.data.model;

import com.fasterxml.jackson.databind.node.ObjectNode;
import com.vladmihalcea.hibernate.type.json.JsonBinaryType;
import lombok.Data;
import org.hibernate.annotations.Type;
import org.hibernate.annotations.TypeDef;

import javax.persistence.*;
import java.util.List;
import java.util.UUID;

@Data
@Entity
@TypeDef(name = "jsonb", typeClass = JsonBinaryType.class)
public class ComponentJobConfig {
    @Id
    @GeneratedValue
    private Long id;
    private String componentId;
    private UUID jobId;
    @ElementCollection
    @CollectionTable(name="component_job_config_output_channels")
    private List<String> outputChannelNames;
    @Type(type = "jsonb")
    @Column(columnDefinition = "jsonb")
    private ObjectNode attributes;
}
