package cz.cuni.mff.socneto.storage.internal.api.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobDto;

import java.util.List;
import java.util.UUID;

public interface JobDtoService {

    JobDto find(UUID id);

    List<JobDto> findAll();

    List<JobDto> findAllByUser(String user);

    JobDto save(JobDto job);

    JobDto update(JobDto job);

    void delete(UUID id);
}
