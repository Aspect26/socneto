package cz.cuni.mff.socneto.storage.internal.repository;

import cz.cuni.mff.socneto.storage.internal.data.model.ComponentJobMetadata;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface ComponentJobMetadataRepository extends CrudRepository<ComponentJobMetadata, Long> {

    Iterable<ComponentJobMetadata> findAllByComponentId(String componentId);

    ComponentJobMetadata findByComponentIdAndJobId(String componentId, UUID jobId);

}
