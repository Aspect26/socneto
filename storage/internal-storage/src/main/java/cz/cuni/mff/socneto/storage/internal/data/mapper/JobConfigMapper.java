package cz.cuni.mff.socneto.storage.internal.data.mapper;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobConfigDto;
import cz.cuni.mff.socneto.storage.internal.data.model.JobConfig;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

import java.util.List;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface JobConfigMapper {

    JobConfig jobConfigDtoToJobConfig(JobConfigDto jobConfigDto);

    List<JobConfig> jobConfigDtosToJobConfigs(List<JobConfigDto> jobConfigDtos);

    JobConfigDto jobConfigToJobConfigDto(JobConfig jobConfig);

    List<JobConfigDto> jobConfigsToJobConfigDtos(List<JobConfig> jobConfigDtos);
}
