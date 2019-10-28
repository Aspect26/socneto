package cz.cuni.mff.socneto.storage.internal.repository;

import cz.cuni.mff.socneto.storage.internal.data.model.JobConfig;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface JobConfigRepository extends CrudRepository<JobConfig, UUID> {

}
