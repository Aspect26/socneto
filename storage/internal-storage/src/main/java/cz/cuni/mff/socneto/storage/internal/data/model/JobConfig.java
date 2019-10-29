package cz.cuni.mff.socneto.storage.internal.data.model;

import lombok.Data;

import javax.persistence.*;
import java.util.List;
import java.util.UUID;

@Data
@Entity
public class JobConfig {
    @Id
    private UUID jobId;
    @ElementCollection
    @CollectionTable(name = "data_analysers", joinColumns = @JoinColumn(name = "jobId"))
    @Column(name = "dataAnalysers")
    private List<String> dataAnalysers;
    @ElementCollection
    @CollectionTable(name = "data_acquirers", joinColumns = @JoinColumn(name = "jobId"))
    @Column(name = "dataAcquirers")
    private List<String> dataAcquirers;
    private String topicQuery;
    private String status;
}
