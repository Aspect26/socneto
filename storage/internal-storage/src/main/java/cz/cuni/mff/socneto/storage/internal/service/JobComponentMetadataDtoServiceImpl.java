package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobComponentMetadataDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobComponentMetadataDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.JobComponentMetadataMapper;
import cz.cuni.mff.socneto.storage.internal.data.model.JobComponentMetadata;
import cz.cuni.mff.socneto.storage.internal.data.model.JobView;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobComponentMetadataDtoServiceImpl implements JobComponentMetadataDtoService {

    private final JobComponentMetadataService jobComponentMetadataService;
    private final JobComponentMetadataMapper jobComponentMetadataMapper;

    @Override
    public JobComponentMetadataDto find(String componentId, UUID jobId) {
        // TODO validate
        return jobComponentMetadataMapper.jobComponentMetadataToJobComponentMetadataDto(jobComponentMetadataService.find(componentId, jobId));
    }

    @Override
    public List<JobComponentMetadataDto> findUnfinished(String componentId) {
        // TODO validate
        return jobComponentMetadataMapper.jobComponentMetadatasToJobComponentMetadataDtos(toList(jobComponentMetadataService.findUnfinished(componentId)));
    }

    @Override
    public JobComponentMetadataDto save(JobComponentMetadataDto jobComponentMetadata) {
        // TODO validate
        return jobComponentMetadataMapper.jobComponentMetadataToJobComponentMetadataDto(jobComponentMetadataService.save(jobComponentMetadataMapper.jobComponentMetadataDtoToJobComponentMetadata(jobComponentMetadata)));
    }

    @Override
    public JobComponentMetadataDto update(JobComponentMetadataDto jobComponentMetadata) {
        // TODO validate
        return jobComponentMetadataMapper.jobComponentMetadataToJobComponentMetadataDto(jobComponentMetadataService.update(jobComponentMetadataMapper.jobComponentMetadataDtoToJobComponentMetadata(jobComponentMetadata)));
    }


    // TODO remove
    private ArrayList<JobComponentMetadata> toList(Iterable<JobComponentMetadata> jobComponentMetadataList) {
        ArrayList<JobComponentMetadata> list = new ArrayList<>();
        jobComponentMetadataList.iterator().forEachRemaining(list::add);
        return list;
    }
}
