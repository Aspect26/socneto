package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobConfigDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobConfigDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class JobConfigController {

    private final JobConfigDtoService jobConfigDtoService;

    @GetMapping("/job-config/{jobId}")
    public JobConfigDto getJobConfig(@PathVariable UUID jobId) {
        return jobConfigDtoService.find(jobId);
    }

    @PostMapping("/job-config")
    public JobConfigDto saveJobConfig(@RequestBody JobConfigDto job) {
        return jobConfigDtoService.save(job);
    }

    @PutMapping("/job-config/{id}")
    public JobConfigDto updateJobConfig(@PathVariable("id") UUID id, @RequestBody JobConfigDto job) {
        return jobConfigDtoService.update(job);
    }

}
