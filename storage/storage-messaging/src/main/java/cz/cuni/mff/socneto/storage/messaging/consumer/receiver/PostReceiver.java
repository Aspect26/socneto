package cz.cuni.mff.socneto.storage.messaging.consumer.receiver;

import com.fasterxml.jackson.databind.DeserializationFeature;
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
public class PostReceiver {

    private final AnalyzedPostDtoService analyzedPostDtoService;
    private final ObjectMapper objectMapper;

    public PostReceiver(AnalyzedPostDtoService analyzedPostDtoService) {
        this.analyzedPostDtoService = analyzedPostDtoService;
        objectMapper = new ObjectMapper();
        objectMapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
    }

    @KafkaListener(topics = "${app.topic.toDbRaw}")
    public void listenPosts(@Payload String post) throws IOException {

        log.info("Post:" + post);

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
