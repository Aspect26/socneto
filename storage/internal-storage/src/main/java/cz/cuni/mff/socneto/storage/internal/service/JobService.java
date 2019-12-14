package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.model.Job;
import cz.cuni.mff.socneto.storage.internal.repository.JobRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import javax.persistence.EntityNotFoundException;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobService {

    private final JobRepository repository;

    public Job find(UUID id) {
        return repository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("Job with id: " + id + " not found"));
    }

    public Iterable<Job> findAll() {
        return repository.findAll();
    }

    public Iterable<Job> findAllByUser(String user) {
        return repository.findAllByUsername(user);
    }

    public Job save(Job job) {
        return repository.save(job);
    }

    public Job update(Job job) {
        return repository.save(job);
    }

    public void delete(UUID id) {
        repository.deleteById(id);
    }

}
