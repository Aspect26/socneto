package cz.cuni.mff.socneto.storage.internal.data.mapper;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobMetadataDto;
import cz.cuni.mff.socneto.storage.internal.data.model.ComponentJobMetadata;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

import java.util.List;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface ComponentJobMetadataMapper {

    ComponentJobMetadata componentJobMetadataDtoToJobComponentMetadata(ComponentJobMetadataDto componentJobMetadataDto);

    List<ComponentJobMetadata> componentJobMetadataDtosToJobComponentMetadatas(List<ComponentJobMetadataDto> componentJobMetadataDtos);

    ComponentJobMetadataDto componentJobMetadataToJobComponentMetadataDto(ComponentJobMetadata componentJobMetadata);

    List<ComponentJobMetadataDto> componentJobMetadatasToJobComponentMetadataDtos(List<ComponentJobMetadata> componentJobMetadataDtos);
}
