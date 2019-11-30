package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentDto;
import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;
import cz.cuni.mff.socneto.storage.internal.api.service.ComponentDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.ComponentMapper;
import cz.cuni.mff.socneto.storage.internal.data.model.Component;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class ComponentDtoServiceImpl implements ComponentDtoService {

    private final ComponentService componentService;
    private final ComponentMapper componentMapper;

    @Override
    public ComponentDto find(String id) {
        // TODO validate
        return componentMapper.componentToComponentDto(componentService.find(id));
    }

    @Override
    public ComponentDto find(String componentId, UUID jobId) {
        return componentMapper.componentToComponentDto(componentService.find(componentId, jobId));
    }

    @Override
    public List<ComponentDto> getAllByType(ComponentType type) {
        // TODO validate
        return componentMapper.componentsToComponentDtos(toList(componentService.getAllByType(type)));
    }


    @Override
    public ComponentDto save(ComponentDto component) {
        // TODO validate
        return componentMapper.componentToComponentDto(componentService.save(componentMapper.componentDtoToComponent(component)));
    }

    @Override
    public ComponentDto update(ComponentDto component) {
        // TODO validate
        return componentMapper.componentToComponentDto(componentService.update(componentMapper.componentDtoToComponent(component)));
    }

    @Override
    public void delete(String id) {
        // TODO validate
        componentService.delete(id);
    }

    private ArrayList<Component> toList(Iterable<Component> components) {
        ArrayList<Component> list = new ArrayList<>();
        components.iterator().forEachRemaining(list::add);
        return list;
    }

}
