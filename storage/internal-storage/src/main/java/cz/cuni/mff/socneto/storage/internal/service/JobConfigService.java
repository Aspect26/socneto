package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.model.JobConfig;
import cz.cuni.mff.socneto.storage.internal.repository.JobConfigRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import javax.persistence.EntityNotFoundException;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobConfigService {

    private final JobConfigRepository repository;

    public JobConfig find(UUID id) {
        return repository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("JobConfig with id: " + id + " not found"));
    }

    public JobConfig save(JobConfig jobConfig) {
        return repository.save(jobConfig);
    }

    public JobConfig update(JobConfig jobConfig) {
        return repository.save(jobConfig);
    }

    public void delete(UUID id) {
        repository.deleteById(id);
    }

}
