package cz.cuni.mff.socneto.storage.internal.repository;

import cz.cuni.mff.socneto.storage.internal.data.model.ComponentJobConfig;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;
import java.util.UUID;

@Repository
public interface ComponentConfigRepository extends CrudRepository<ComponentJobConfig, Long> {

    Iterable<ComponentJobConfig> findAllByComponentId(String componentId);

    Optional<ComponentJobConfig> findByComponentIdAndJobId(String componentId, UUID jobId);

}
