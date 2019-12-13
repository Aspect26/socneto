package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobConfigDto;
import cz.cuni.mff.socneto.storage.internal.api.service.ComponentJobConfigDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.ComponentConfigMapper;
import cz.cuni.mff.socneto.storage.internal.data.model.ComponentJobConfig;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class ComponentJobJobConfigDtoServiceImpl implements ComponentJobConfigDtoService {

    private final ComponentJobConfigService componentJobConfigService;
    private final ComponentConfigMapper componentConfigMapper;

    @Override
    public ComponentJobConfigDto find(Long id) {
        // TODO validate
        return componentConfigMapper.componentConfigToComponentConfigDto(componentJobConfigService.find(id));
    }

    @Override
    public List<ComponentJobConfigDto> findAllByComponentId(String componentId) {
        return componentConfigMapper.componentConfigsToComponentConfigDtos(toList(componentJobConfigService.findAllByComponentId(componentId)));
    }

    @Override
    public ComponentJobConfigDto find(String componentId, UUID jobId) {
        // TODO validate
        return componentConfigMapper.componentConfigToComponentConfigDto(componentJobConfigService.find(componentId, jobId));
    }

    @Override
    public ComponentJobConfigDto save(ComponentJobConfigDto componentJobConfigDto) {
        // TODO validate
        return componentConfigMapper.componentConfigToComponentConfigDto(
                componentJobConfigService.save(
                        componentConfigMapper.componentConfigDtoToComponentConfig(componentJobConfigDto)));
    }

    @Override
    public ComponentJobConfigDto update(ComponentJobConfigDto componentJobConfigDto) {
        // TODO validate
        return componentConfigMapper.componentConfigToComponentConfigDto(
                componentJobConfigService.update(
                        componentConfigMapper.componentConfigDtoToComponentConfig(componentJobConfigDto)));
    }


    // TODO remove
    private ArrayList<ComponentJobConfig> toList(Iterable<ComponentJobConfig> jobComponentMetadataList) {
        ArrayList<ComponentJobConfig> list = new ArrayList<>();
        jobComponentMetadataList.iterator().forEachRemaining(list::add);
        return list;
    }
}
