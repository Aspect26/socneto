package cz.cuni.mff.socneto.storage.internal.data.model;

import com.fasterxml.jackson.databind.node.ObjectNode;
import com.vladmihalcea.hibernate.type.json.JsonBinaryType;
import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;
import lombok.Data;
import org.hibernate.annotations.Type;
import org.hibernate.annotations.TypeDef;

import javax.persistence.*;
import java.util.UUID;

@Data
@Entity
@TypeDef(name = "jsonb", typeClass = JsonBinaryType.class)
public class Component { // TODO rename Component configuration
    @Id
    @GeneratedValue
    private Long id;
    @Column(nullable = false)
    private String componentId;
    @Column(nullable = false)
    private UUID jobId;
    @Enumerated(EnumType.STRING)
    private ComponentType type;
    private String inputChannelName;
    private String updateChannelName;
    @Type(type = "jsonb")
    @Column(columnDefinition = "jsonb")
    private ObjectNode attributes;
}
