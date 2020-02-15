package cz.cuni.mff.socneto.storage.internal.api.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import java.util.Date;
import java.util.UUID;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class JobDto {
    @NotNull
    private UUID jobId;
    @NotBlank
    private String jobName;
    @NotBlank
    private String username;
    private String topicQuery;
    private String status;
    private String language;
    private Date startedAt;
    private Date finishedAt;
}
