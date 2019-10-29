package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobConfigDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobConfigDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.JobConfigMapper;
import cz.cuni.mff.socneto.storage.internal.data.model.JobConfig;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobConfigDtoServiceImpl implements JobConfigDtoService {

    private final JobConfigService jobConfigService;
    private final JobConfigMapper jobConfigMapper;

    @Override
    public JobConfigDto find(UUID id) {
        // TODO validate
        return jobConfigMapper.jobConfigToJobConfigDto(jobConfigService.find(id));
    }

    @Override
    public JobConfigDto save(JobConfigDto jobConfig) {
        // TODO validate
        return jobConfigMapper.jobConfigToJobConfigDto(jobConfigService.save(jobConfigMapper.jobConfigDtoToJobConfig(jobConfig)));
    }

    @Override
    public JobConfigDto update(JobConfigDto jobConfig) {
        // TODO validate
        return jobConfigMapper.jobConfigToJobConfigDto(jobConfigService.update(jobConfigMapper.jobConfigDtoToJobConfig(jobConfig)));
    }

    @Override
    public void delete(UUID id) {
        // TODO validate
        jobConfigService.delete(id);
    }

}
