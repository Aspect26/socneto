package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobDto;
import org.junit.Assert;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.junit4.SpringRunner;

import java.time.Instant;
import java.util.Date;
import java.util.List;
import java.util.UUID;

@RunWith(SpringRunner.class)
@SpringBootTest
public class JobControllerTest {

    @Autowired
    private JobController jobController;

    @Test
    public void testSingleJob() {
        var job = JobDto.builder()
                .jobId(UUID.randomUUID())
                .jobName("name")
                .language("language")
                .startedAt(new Date())
                .status("status")
                .username("ADMIN")
                .build();

        JobDto createdJob = jobController.saveJob(job);

        JobDto getJob = jobController.getJob(job.getJobId());

        Assert.assertEquals(job, createdJob);
        Assert.assertEquals(job, getJob);
    }

    @Test
    public void testListJobs() {
        var job1 = JobDto.builder()
                .jobId(UUID.randomUUID())
                .jobName("name")
                .language("language")
                .startedAt(Date.from(Instant.now().minusSeconds(100)))
                .status("status")
                .username(UUID.randomUUID().toString())
                .build();

        var job2 = JobDto.builder()
                .jobId(UUID.randomUUID())
                .jobName("name")
                .language("language")
                .startedAt(Date.from(Instant.now()))
                .status("status")
                .username(UUID.randomUUID().toString())
                .build();

        JobDto createdJob1 = jobController.saveJob(job1);
        JobDto createdJob2 = jobController.saveJob(job2);

        Assert.assertEquals(job1, createdJob1);
        Assert.assertEquals(job2, createdJob2);

        List<JobDto> allJobs = jobController.getJobsByUsername(null);

        Assert.assertTrue(allJobs.contains(createdJob1));
        Assert.assertTrue(allJobs.contains(createdJob2));
    }

    @Test
    public void testListJobsByUser() {
        var job1 = JobDto.builder()
                .jobId(UUID.randomUUID())
                .jobName("name")
                .language("language")
                .startedAt(Date.from(Instant.now().minusSeconds(100)))
                .status("status")
                .username(UUID.randomUUID().toString())
                .build();

        var job2 = JobDto.builder()
                .jobId(UUID.randomUUID())
                .jobName("name")
                .language("language")
                .startedAt(Date.from(Instant.now()))
                .status("status")
                .username(UUID.randomUUID().toString())
                .build();

        JobDto createdJob1 = jobController.saveJob(job1);
        JobDto createdJob2 = jobController.saveJob(job2);

        Assert.assertEquals(job1, createdJob1);
        Assert.assertEquals(job2, createdJob2);

        List<JobDto> allJobs = jobController.getJobsByUsername(job2.getUsername());

        Assert.assertTrue(allJobs.size() == 1);
        Assert.assertTrue(allJobs.contains(job2));
    }
}