package cz.cuni.mff.socneto.storage.internal.data.mapper;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobConfigDto;
import cz.cuni.mff.socneto.storage.internal.data.model.ComponentJobConfig;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

import java.util.List;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface ComponentConfigMapper {

    ComponentJobConfig componentConfigDtoToComponentConfig(ComponentJobConfigDto componentJobConfigDto);

    List<ComponentJobConfig> componentConfigDtosToComponentConfigs(List<ComponentJobConfigDto> componentJobConfigDtos);

    ComponentJobConfigDto componentConfigToComponentConfigDto(ComponentJobConfig componentJobConfig);

    List<ComponentJobConfigDto> componentConfigsToComponentConfigDtos(List<ComponentJobConfig> componentJobConfigDtos);
}
