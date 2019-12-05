package cz.cuni.mff.socneto.storage.internal.api.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobConfigDto;

import java.util.List;
import java.util.UUID;

public interface ComponentJobConfigDtoService {

    ComponentJobConfigDto find(Long id);

    List<ComponentJobConfigDto> findAllByComponentId(String componentId);

    ComponentJobConfigDto find(String componentId, UUID jobId);

    ComponentJobConfigDto save(ComponentJobConfigDto componentJobConfigDto);

    ComponentJobConfigDto update(ComponentJobConfigDto componentJobConfigDto);

}
