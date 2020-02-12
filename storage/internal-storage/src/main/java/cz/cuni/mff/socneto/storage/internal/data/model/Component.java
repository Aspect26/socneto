package cz.cuni.mff.socneto.storage.internal.data.model;

import com.fasterxml.jackson.databind.node.ObjectNode;
import com.vladmihalcea.hibernate.type.json.JsonBinaryType;
import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;
import lombok.Data;
import org.hibernate.annotations.Type;
import org.hibernate.annotations.TypeDef;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.Id;

@Data
@Entity
@TypeDef(name = "jsonb", typeClass = JsonBinaryType.class)
public class Component {
    @Id
    private String componentId;
    @Enumerated(EnumType.STRING)
    private ComponentType type;
    @Column(nullable = false)
    private String inputChannelName;
    @Column(nullable = false)
    private String updateChannelName;
    @Type(type = "jsonb")
    @Column(columnDefinition = "jsonb")
    private ObjectNode attributes;
}
