package cz.cuni.mff.socneto.storage.messaging.consumer.receiver;

import com.fasterxml.jackson.databind.ObjectMapper;
import cz.cuni.mff.socneto.storage.internal.api.dto.JobDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobDtoService;
import cz.cuni.mff.socneto.storage.messaging.consumer.model.JobMessage;
import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Service;

import java.io.IOException;

@Slf4j
@Service
@AllArgsConstructor
public class JobReceiver {

    private final JobDtoService jobDtoService;

    private final ObjectMapper objectMapper = new ObjectMapper();

//    @KafkaListener(topics = "${app.topic.toDbJob}")
    public void listenPosts(@Payload String job) throws IOException {

        JobMessage jobMessage = objectMapper.readValue(job, JobMessage.class);

        var jobDto = JobDto.builder().jobId(jobMessage.getJobId()).build();

        jobDtoService.save(jobDto);
    }
}
