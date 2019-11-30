package cz.cuni.mff.socneto.storage.internal.api.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentDto;
import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;

import java.util.List;
import java.util.UUID;

public interface ComponentDtoService {

    ComponentDto find(String id);

    ComponentDto find(String componentId, UUID jobId);

    List<ComponentDto> getAllByType(ComponentType type);

    ComponentDto save(ComponentDto component);

    ComponentDto update(ComponentDto component);

    void delete(String id);
}
