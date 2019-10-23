package cz.cuni.mff.socneto.storage.internal.api.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.Date;
import java.util.UUID;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class JobDto {
    private UUID jobId;
    private String username;
    private String jobName;
    private boolean hasFinished;
    private Date startedAt;
    private Date finishedAt;
}
