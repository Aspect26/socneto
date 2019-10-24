package cz.cuni.mff.socneto.storage.internal.api.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobViewDto;

import java.util.UUID;

public interface JobViewDtoService {

    JobViewDto find(UUID id);

    JobViewDto save(JobViewDto jobView);

    JobViewDto update(JobViewDto jobView);

    void delete(UUID id);
}
