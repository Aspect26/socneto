package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobConfigDto;
import cz.cuni.mff.socneto.storage.internal.api.service.ComponentJobConfigDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.ComponentConfigMapper;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class ComponentJobJobConfigDtoServiceImpl implements ComponentJobConfigDtoService {

    private final ComponentJobConfigService componentJobConfigService;
    private final ComponentConfigMapper componentConfigMapper;

    @Override
    public ComponentJobConfigDto find(Long id) {
        return componentConfigMapper.componentConfigToComponentConfigDto(componentJobConfigService.find(id));
    }

    @Override
    public List<ComponentJobConfigDto> findAllByComponentId(String componentId) {
        return componentConfigMapper.componentConfigsToComponentConfigDtos(
                ServiceUtils.toList(componentJobConfigService.findAllByComponentId(componentId)));
    }

    @Override
    public ComponentJobConfigDto find(String componentId, UUID jobId) {
        return componentConfigMapper.componentConfigToComponentConfigDto(componentJobConfigService.find(componentId, jobId));
    }

    @Override
    public ComponentJobConfigDto save(ComponentJobConfigDto componentJobConfigDto) {
        return componentConfigMapper.componentConfigToComponentConfigDto(
                componentJobConfigService.save(
                        componentConfigMapper.componentConfigDtoToComponentConfig(componentJobConfigDto)));
    }

    @Override
    public ComponentJobConfigDto update(ComponentJobConfigDto componentJobConfigDto) {
        return componentConfigMapper.componentConfigToComponentConfigDto(
                componentJobConfigService.update(
                        componentConfigMapper.componentConfigDtoToComponentConfig(componentJobConfigDto)));
    }
}
