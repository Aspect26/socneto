package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobViewDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobViewDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import javax.websocket.server.PathParam;
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
        return jobViewDtoService.save(jobViewDto);
    }

    @PutMapping("/jobs/{jobId}/view")
    public JobViewDto updateJobView(@PathVariable("jobId") UUID jobId, @RequestBody JobViewDto jobViewDto) {
        return jobViewDtoService.update(jobViewDto);
    }

    @DeleteMapping("/jobs/{jobId}/view")
    public void getJob(@PathVariable("jobId") UUID jobId) {
        jobViewDtoService.delete(jobId);
    }
}
