package cz.cuni.mff.socneto.storage.messaging.consumer.model;

import lombok.Data;

import java.util.Date;
import java.util.UUID;

@Data
public class JobMessage {
    private UUID jobId;
    private String username;
    private String jobName;
    private boolean hasFinished;
    private Date startedAt;
    private Date finishedAt;
}
