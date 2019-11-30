package cz.cuni.mff.socneto.storage.internal.data.model;

import lombok.Data;

import javax.persistence.*;
import java.util.Date;
import java.util.List;
import java.util.UUID;

@Data
@Entity
public class Job {
    @Id
    private UUID jobId;
    private String jobName;
    @Column(name = "userId")
    private String user;
    private String topicQuery;
    private String status;
    private String language;
    private Date startedAt;
    private Date finishedAt;

    @OneToMany(targetEntity = Component.class, cascade = CascadeType.ALL, orphanRemoval = true)
    @JoinColumn(name = "jobId")
    private List<Component> componentConfigs; //TODO rename component
}
