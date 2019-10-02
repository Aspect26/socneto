package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.dto.JobDto;
import cz.cuni.mff.socneto.storage.internal.repository.InternalRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.Set;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobService {

    private final InternalRepository<JobDto> repository;

    public JobDto getJob(UUID id) {
        return repository.getByPredicate(job -> job.getJobId().equals(id)).get();
    }

    public Set<JobDto> getManyJobsByUserId(String userId) {
        return repository.getManyByPredicate(job -> job.getUsername().equals(userId));
    }

    public JobDto saveJob(JobDto job) {
        repository.getByPredicate(j -> j.getJobId().equals(job.getJobId())).ifPresent(repository::delete);
        return repository.save(job);
    }

}
