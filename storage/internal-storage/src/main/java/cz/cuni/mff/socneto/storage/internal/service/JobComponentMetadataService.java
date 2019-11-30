package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.model.JobComponentMetadata;
import cz.cuni.mff.socneto.storage.internal.repository.JobComponentMetadataRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobComponentMetadataService {

    private final JobComponentMetadataRepository repository;

    public JobComponentMetadata find(String componentId, UUID jobId) {
        return repository.findByComponentIdAndJobId(componentId, jobId);
    }

    public Iterable<JobComponentMetadata> findUnfinished(String componentId) {
        return repository.findAllByComponentIdAndFinishedFalse(componentId);
    }

    public JobComponentMetadata save(JobComponentMetadata jobComponentMetadata) {
        return repository.save(jobComponentMetadata);
    }

    public JobComponentMetadata update(JobComponentMetadata jobComponentMetadata) {
        var old = find(jobComponentMetadata.getComponentId(), jobComponentMetadata.getJobId());
        jobComponentMetadata.setId(old.getId());
        return repository.save(jobComponentMetadata);
    }

}
