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
public class JobComponentMetadata {
    @Id
    private UUID id;
    private String componentId;
    private UUID jobId;
    private boolean finished;
    @Type(type = "jsonb")
    @Column(columnDefinition = "jsonb")
    private ObjectNode componentAttributes;
    @Type(type = "jsonb")
    @Column(columnDefinition = "jsonb")
    private ObjectNode jobAttributes;
    @Type(type = "jsonb")
    @Column(columnDefinition = "jsonb")
    private ObjectNode componentMetadata;
}
