package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobDto;
import cz.cuni.mff.socneto.storage.internal.api.service.ComponentJobConfigDtoService;
import cz.cuni.mff.socneto.storage.internal.api.service.JobDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.JobMapper;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobDtoServiceImpl implements JobDtoService {

    private final JobService jobService;
    private final JobMapper jobMapper;

    private final ComponentJobConfigDtoService componentJobConfigDtoService;

    @Override
    public JobDto find(UUID id) {
        return jobMapper.jobToJobDto(jobService.find(id));
    }

    @Override
    public List<JobDto> findAll() {
        return jobMapper.jobsToJobDtos(ServiceUtils.toList(jobService.findAll()));
    }

    @Override
    public List<JobDto> findAllByUser(String user) {
        return jobMapper.jobsToJobDtos(ServiceUtils.toList(jobService.findAllByUser(user)));
    }

    @Override
    public JobDto save(JobDto job) {
        return jobMapper.jobToJobDto(jobService.save(jobMapper.jobDtoToJob(job)));
    }

    @Override
    public JobDto update(JobDto job) {
        return jobMapper.jobToJobDto(jobService.update(jobMapper.jobDtoToJob(job)));
    }
}
