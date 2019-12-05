package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobViewDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobViewDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.JobViewMapper;
import cz.cuni.mff.socneto.storage.internal.data.model.JobView;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class JobViewDtoServiceImpl implements JobViewDtoService {

    private final JobViewService jobViewService;
    private final JobViewMapper jobViewMapper;

    @Override
    public JobViewDto find(UUID id) {
        // TODO validate
        return jobViewMapper.jobViewToJobViewDto(jobViewService.find(id));
    }

    @Override
    public JobViewDto save(JobViewDto jobView) {
        // TODO validate
        return jobViewMapper.jobViewToJobViewDto(jobViewService.save(jobViewMapper.jobViewDtoToJobView(jobView)));
    }

    @Override
    public JobViewDto update(JobViewDto jobView) {
        // TODO validate
        return jobViewMapper.jobViewToJobViewDto(jobViewService.update(jobViewMapper.jobViewDtoToJobView(jobView)));
    }

    private ArrayList<JobView> toList(Iterable<JobView> jobViews) {
        ArrayList<JobView> list = new ArrayList<>();
        jobViews.iterator().forEachRemaining(list::add);
        return list;
    }

}
