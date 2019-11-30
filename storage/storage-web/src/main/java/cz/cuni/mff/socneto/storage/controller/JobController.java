package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import javax.validation.Valid;
import javax.websocket.server.PathParam;
import java.util.List;
import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class JobController {

    private final JobDtoService jobDtoService;

    @GetMapping("/jobs")
    public List<JobDto> getJobsByUser(@RequestParam("userId") String username) {
        return jobDtoService.findAllByUsername(username);
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
    public JobDto updateJob(@PathVariable("id") UUID id, @RequestBody JobDto job) {
        return jobDtoService.update(job);
    }

}
