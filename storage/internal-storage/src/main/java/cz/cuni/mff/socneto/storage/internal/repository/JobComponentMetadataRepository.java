package cz.cuni.mff.socneto.storage.internal.repository;

import cz.cuni.mff.socneto.storage.internal.data.model.JobComponentMetadata;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public interface JobComponentMetadataRepository extends CrudRepository<JobComponentMetadata, UUID> {

    JobComponentMetadata findByComponentIdAndJobId(String componentId, UUID jobId);

    Iterable<JobComponentMetadata> findAllByComponentIdAndFinishedFalse(String componentId);

}
