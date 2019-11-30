package cz.cuni.mff.socneto.storage.internal.repository;

import cz.cuni.mff.socneto.storage.internal.data.model.Job;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface JobRepository extends CrudRepository<Job, UUID> {

    Iterable<Job> findAllByUser(String user);
    
}
