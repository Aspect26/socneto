package cz.cuni.mff.socneto.storage.internal.controller;

import cz.cuni.mff.socneto.storage.internal.data.dto.JobDto;
import cz.cuni.mff.socneto.storage.internal.data.dto.UserDto;
import cz.cuni.mff.socneto.storage.internal.service.JobService;
import cz.cuni.mff.socneto.storage.internal.service.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import javax.websocket.server.PathParam;
import java.util.Set;
import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class InternalController {

    private final UserService userService;
    private final JobService jobService;

    @GetMapping("/users")
    public UserDto getUserById(@PathParam("userId") String userId) {
        return userService.getUserById(userId);
    }

    @PostMapping("/users")
    public UserDto saveUser(@RequestBody UserDto user) {
        return userService.saveUser(user);
    }

    @GetMapping("/jobs")
    public Set<JobDto> getJobsByUser(@PathParam("userId") String userId) {
        return jobService.getManyJobsByUserId(userId);
    }

    @GetMapping("/jobs/{id}")
    public JobDto getJob(@PathVariable UUID id) {
        return jobService.getJob(id);
    }

    @PostMapping("/jobs")
    public JobDto saveJob(@RequestBody JobDto job) {
        return jobService.saveJob(job);
    }

}
