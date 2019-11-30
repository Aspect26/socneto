package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.JobMapper;
import cz.cuni.mff.socneto.storage.internal.data.model.Job;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.validation.ValidationUtils;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobDtoServiceImpl implements JobDtoService {

    private final JobService jobService;
    private final JobMapper jobMapper;

    @Override
    public JobDto find(UUID id) {
        // TODO validate
        return jobMapper.jobToJobDto(jobService.find(id));
    }

    @Override
    public List<JobDto> findAll() {
        return jobMapper.jobsToJobDtos(toList(jobService.findAll()));
    }

    @Override
    public List<JobDto> findAllByUsername(String username) {
        // TODO validate
        return jobMapper.jobsToJobDtos(toList(jobService.findAllByUsername(username)));
    }

    @Override
    public JobDto save(JobDto job) {
        // TODO validate
        return jobMapper.jobToJobDto(jobService.save(jobMapper.jobDtoToJob(job)));
    }

    @Override
    public JobDto update(JobDto job) {
        // TODO validate
        return jobMapper.jobToJobDto(jobService.update(jobMapper.jobDtoToJob(job)));
    }

    @Override
    public void delete(UUID id) {
        // TODO validate
        jobService.delete(id);
    }

    // TODO remove
    private ArrayList<Job> toList(Iterable<Job> jobs) {
        ArrayList<Job> list = new ArrayList<>();
        jobs.iterator().forEachRemaining(list::add);
        return list;
    }

}
