package cz.cuni.mff.socneto.storage.internal.data.mapper;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobComponentMetadataDto;
import cz.cuni.mff.socneto.storage.internal.data.model.JobComponentMetadata;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

import java.util.List;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface JobComponentMetadataMapper {

    JobComponentMetadata jobComponentMetadataDtoToJobComponentMetadata(JobComponentMetadataDto jobComponentMetadataDto);

    List<JobComponentMetadata> jobComponentMetadataDtosToJobComponentMetadatas(List<JobComponentMetadataDto> jobComponentMetadataDtos);

    JobComponentMetadataDto jobComponentMetadataToJobComponentMetadataDto(JobComponentMetadata jobComponentMetadata);

    List<JobComponentMetadataDto> jobComponentMetadatasToJobComponentMetadataDtos(List<JobComponentMetadata> jobComponentMetadataDtos);
}
