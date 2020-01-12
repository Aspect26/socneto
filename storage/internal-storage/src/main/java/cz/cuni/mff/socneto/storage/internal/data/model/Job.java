package cz.cuni.mff.socneto.storage.internal.data.model;

import lombok.Data;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import java.util.Date;
import java.util.UUID;

@Data
@Entity
public class Job {
    @Id
    private UUID jobId;
    private String jobName;
    @Column(name = "userId")
    private String username;
    private String topicQuery;
    private String status;
    private String language;
    private Date startedAt;
    private Date finishedAt;
}
