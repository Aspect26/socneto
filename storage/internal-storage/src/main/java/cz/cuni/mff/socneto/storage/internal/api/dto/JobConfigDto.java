package cz.cuni.mff.socneto.storage.internal.api.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import javax.persistence.*;
import java.util.List;
import java.util.UUID;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class JobConfigDto {
    private UUID jobId;
    private List<String> dataAnalysers;
    private List<String> dataAcquirers;
    private String topicQuery;
    private String status;
}
