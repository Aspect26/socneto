package cz.cuni.mff.socneto.storage.internal.data.mapper;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobViewDto;
import cz.cuni.mff.socneto.storage.internal.data.model.JobView;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

import java.util.List;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface JobViewMapper {

    JobView jobViewDtoToJobView(JobViewDto jobViewDto);

    List<JobView> jobViewDtosToJobViews(List<JobViewDto> jobViewDtos);

    JobViewDto jobViewToJobViewDto(JobView jobView);

    List<JobViewDto> jobViewsToJobViewDtos(List<JobView> jobViewDtos);
}
