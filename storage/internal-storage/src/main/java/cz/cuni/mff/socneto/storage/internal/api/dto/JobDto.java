package cz.cuni.mff.socneto.storage.internal.api.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import javax.validation.constraints.NotBlank;
import java.util.Date;
import java.util.UUID;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class JobDto {
    private UUID jobId;
    @NotBlank(message = "Job name can't be null.")
    private String jobName;
    @NotBlank(message = "User can't be null.")
    private String username;
    private String topicQuery;
    private String status;
    private String language;
    private Date startedAt;
    private Date finishedAt;
}
