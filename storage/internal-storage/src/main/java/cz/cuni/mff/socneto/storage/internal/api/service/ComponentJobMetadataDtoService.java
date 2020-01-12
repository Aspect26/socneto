package cz.cuni.mff.socneto.storage.internal.api.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobMetadataDto;

import java.util.List;
import java.util.UUID;

public interface ComponentJobMetadataDtoService {

    List<ComponentJobMetadataDto> findAllByComponentId(String componentId);

    ComponentJobMetadataDto find(String componentId, UUID jobId);

    ComponentJobMetadataDto save(ComponentJobMetadataDto componentJobMetadataDto);

    ComponentJobMetadataDto update(ComponentJobMetadataDto componentJobMetadataDto);
}
