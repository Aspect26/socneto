package cz.cuni.mff.socneto.storage.internal.data.model;

import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.Data;
import org.hibernate.annotations.Type;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import java.util.UUID;

@Data
@Entity
public class ComponentJobMetadata {
    @Id
    private Long id;
    private String componentId;
    private UUID jobId;
    @Type(type = "jsonb")
    @Column(columnDefinition = "jsonb")
    private ObjectNode componentMetadata;
}
