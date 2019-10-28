package cz.cuni.mff.socneto.storage.internal.api.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobConfigDto;

import java.util.UUID;

public interface JobConfigDtoService {

    JobConfigDto find(UUID id);

    JobConfigDto save(JobConfigDto jobConfig);

    JobConfigDto update(JobConfigDto jobConfig);

    void delete(UUID id);
}
