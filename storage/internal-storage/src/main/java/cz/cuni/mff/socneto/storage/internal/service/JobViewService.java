package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.model.JobView;
import cz.cuni.mff.socneto.storage.internal.repository.JobViewRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import javax.persistence.EntityNotFoundException;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobViewService {

    private final JobViewRepository repository;

    public JobView find(UUID id) {
        return repository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("JobView with id: " + id + " not found"));
    }

    public JobView save(JobView jobView) {
        return repository.save(jobView);
    }

    public JobView update(JobView jobView) {
        return repository.save(jobView);
    }

    public void delete(UUID id) {
        repository.deleteById(id);
    }

}
