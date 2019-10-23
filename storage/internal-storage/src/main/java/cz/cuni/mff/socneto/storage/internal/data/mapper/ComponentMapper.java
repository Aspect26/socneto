package cz.cuni.mff.socneto.storage.internal.data.mapper;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentDto;
import cz.cuni.mff.socneto.storage.internal.data.model.Component;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

import java.util.List;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface ComponentMapper {

    Component componentDtoToComponent(ComponentDto componentDto);

    List<Component> componentDtosToComponents(List<ComponentDto> componentDtos);

    ComponentDto componentToComponentDto(Component component);

    List<ComponentDto> componentsToComponentDtos(List<Component> componentDtos);
}
