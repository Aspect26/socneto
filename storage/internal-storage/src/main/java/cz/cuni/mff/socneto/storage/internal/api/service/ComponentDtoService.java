package cz.cuni.mff.socneto.storage.internal.api.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentDto;
import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;

import java.util.List;

public interface ComponentDtoService {

    ComponentDto find(String id);

    List<ComponentDto> findAll();

    List<ComponentDto> findAllByType(ComponentType type);

    ComponentDto save(ComponentDto component);

    ComponentDto update(ComponentDto component);

}
