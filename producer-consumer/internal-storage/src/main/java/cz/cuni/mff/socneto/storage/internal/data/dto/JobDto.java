package cz.cuni.mff.socneto.storage.internal.data.dto;

import lombok.Value;

import java.util.Date;
import java.util.UUID;

@Value
public class JobDto {
    private UUID jobId;
    private String username;
    private String jobName;
    private boolean hasFinished;
    private Date startedAt;
    private Date finishedAt;
}
