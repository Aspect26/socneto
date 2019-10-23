package cz.cuni.mff.socneto.storage.internal.data.mapper;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobDto;
import cz.cuni.mff.socneto.storage.internal.data.model.Job;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

import java.util.List;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface JobMapper {

    Job jobDtoToJob(JobDto jobDto);

    List<Job> jobDtosToJobs(List<JobDto> jobDtos);

    JobDto jobToJobDto(Job job);

    List<JobDto> jobsToJobDtos(List<Job> jobDtos);
}
