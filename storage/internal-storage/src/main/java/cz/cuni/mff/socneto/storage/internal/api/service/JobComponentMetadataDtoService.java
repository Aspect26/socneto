package cz.cuni.mff.socneto.storage.internal.api.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobComponentMetadataDto;

import java.util.List;
import java.util.UUID;

public interface JobComponentMetadataDtoService {

    JobComponentMetadataDto find(String componentId, UUID jobId);

    List<JobComponentMetadataDto> findUnfinished(String componentId);

    JobComponentMetadataDto save(JobComponentMetadataDto jobComponentMetadataDto);

    JobComponentMetadataDto update(JobComponentMetadataDto jobComponentMetadataDto);
}
