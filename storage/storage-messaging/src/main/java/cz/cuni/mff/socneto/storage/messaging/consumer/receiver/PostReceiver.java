package cz.cuni.mff.socneto.storage.messaging.consumer.receiver;

import com.fasterxml.jackson.databind.ObjectMapper;
import cz.cuni.mff.socneto.storage.analysis.storage.api.dto.AnalyzedObjectDto;
import cz.cuni.mff.socneto.storage.analysis.storage.api.dto.PostDto;
import cz.cuni.mff.socneto.storage.analysis.storage.api.service.AnalyzedPostDtoService;
import cz.cuni.mff.socneto.storage.messaging.consumer.model.AnalysisMessage;
import cz.cuni.mff.socneto.storage.messaging.consumer.model.PostMessage;
import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Service;

import java.io.IOException;

@Slf4j
@Service
@AllArgsConstructor
public class PostReceiver {

    private final AnalyzedPostDtoService analyzedPostDtoService;

    private final ObjectMapper objectMapper = new ObjectMapper();

    @KafkaListener(topics = "${app.topic.toDbRaw}")
    public void listenPosts(@Payload String post) throws IOException {

        PostMessage postMessage = objectMapper.readValue(post, PostMessage.class);

        var postDto = PostDto.builder()
                .id(postMessage.getJobId().toString())
                .authorId(postMessage.getAuthorId())
                .text(postMessage.getText())
                .source(postMessage.getSource())
                .dateTime(postMessage.getDateTime().toString())
                .build();

        var analyzedObject = AnalyzedObjectDto.<PostDto, String>builder()
                .post(postDto)
                .id(postMessage.getPostId())
                .jobId(postMessage.getJobId())
                .build();

        analyzedPostDtoService.create(analyzedObject);
    }
}
