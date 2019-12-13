package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import javax.validation.Valid;
import java.util.List;
import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class JobController {

    private final JobDtoService jobDtoService;

    @GetMapping("/jobs")
    public List<JobDto> getJobsByUsername(@RequestParam(value = "username", required = false) String username) {
        if (username == null) {
            return jobDtoService.findAll();
        } else {
            return jobDtoService.findAllByUser(username);
        }
    }

    @GetMapping("/jobs/{id}")
    public JobDto getJob(@PathVariable UUID id) {
        return jobDtoService.find(id);
    }

    @PostMapping("/jobs")
    public JobDto saveJob(@Valid @RequestBody JobDto job) {
        return jobDtoService.save(job);
    }

    @PutMapping("/jobs/{id}")
    public JobDto updateJob(@PathVariable UUID id, @RequestBody JobDto job) {
        job.setJobId(id);
        return jobDtoService.update(job);
    }

}
