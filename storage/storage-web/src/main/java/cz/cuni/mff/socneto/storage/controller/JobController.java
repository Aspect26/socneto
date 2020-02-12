package cz.cuni.mff.socneto.storage.controller;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import cz.cuni.mff.socneto.storage.internal.api.dto.JobDto;
import cz.cuni.mff.socneto.storage.internal.api.dto.JobViewDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobDtoService;
import cz.cuni.mff.socneto.storage.internal.api.service.JobViewDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import javax.validation.Valid;
import java.io.IOException;
import java.util.List;
import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class JobController {

    private final JobDtoService jobDtoService;
    private final JobViewDtoService jobViewDtoService;

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
        JobViewDto jobViewDto = new JobViewDto();
        jobViewDto.setJobId(job.getJobId());
        try {
            jobViewDto.setViewConfiguration((ObjectNode) new ObjectMapper().readTree("{ \"chartDefinitions\": []}"));
        } catch (IOException e) {
            e.printStackTrace();
        }
        jobViewDtoService.save(jobViewDto);

        return jobDtoService.save(job);
    }

    @PutMapping("/jobs/{id}")
    public JobDto updateJob(@PathVariable UUID id, @Valid @RequestBody JobDto job) {
        job.setJobId(id);
        return jobDtoService.update(job);
    }

}
