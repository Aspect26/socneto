package cz.cuni.mff.socneto.storage.internal.data.model;

import lombok.Data;

import javax.persistence.Entity;
import javax.persistence.Id;
import java.util.Date;
import java.util.UUID;

@Data
@Entity
public class Job {
    @Id
    private UUID jobId;
    private String username;
    private String jobName;
    private boolean hasFinished;
    private Date startedAt;
    private Date finishedAt;
}
