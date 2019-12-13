package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobViewDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobViewDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class JobViewController {

    private final JobViewDtoService jobViewDtoService;

    @GetMapping("/jobs/{jobId}/view")
    public JobViewDto getJobView(@PathVariable("jobId") UUID jobId) {
        return jobViewDtoService.find(jobId);
    }

    @PostMapping("/jobs/{jobId}/view")
    public JobViewDto createJobView(@PathVariable("jobId") UUID jobId, @RequestBody JobViewDto jobViewDto) {
        jobViewDto.setJobId(jobId);
        return jobViewDtoService.save(jobViewDto);
    }

    @PutMapping("/jobs/{jobId}/view")
    public JobViewDto updateJobView(@PathVariable("jobId") UUID jobId, @RequestBody JobViewDto jobViewDto) {
        jobViewDto.setJobId(jobId);
        return jobViewDtoService.update(jobViewDto);
    }
}
