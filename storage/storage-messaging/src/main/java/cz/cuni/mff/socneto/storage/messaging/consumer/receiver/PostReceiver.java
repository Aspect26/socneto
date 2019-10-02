package cz.cuni.mff.socneto.storage.messaging.consumer.receiver;

import com.fasterxml.jackson.databind.ObjectMapper;
import cz.cuni.mff.socneto.storage.analysis.data.dto.AnalyzedObjectDto;
import cz.cuni.mff.socneto.storage.analysis.data.dto.PostDto;
import cz.cuni.mff.socneto.storage.analysis.service.AnalyzedPostService;
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

    private final AnalyzedPostService analyzedPostService;

    private final ObjectMapper objectMapper = new ObjectMapper();

    @KafkaListener(topics = "${app.topic.toDbRaw}")
    public void listenPosts(@Payload String post) throws IOException {

        PostMessage postMessage = objectMapper.readValue(post, PostMessage.class);

        var postDto = PostDto.builder()
                .id(postMessage.getJobId().toString())
                .authorId(postMessage.getAuthorId())
                .text(postMessage.getText())
                .source(postMessage.getSource())
                .dateTime(postMessage.getDateTime())
                .build();

        var analyzedObject = AnalyzedObjectDto.<PostDto, String>builder()
                .post(postDto)
                .id(postMessage.getPostId())
                .jobId(postMessage.getJobId())
                .build();

        analyzedPostService.create(analyzedObject);
    }


    @KafkaListener(topics = "${app.topic.toDbAnalyzed}")
    public void listenAnalyzedPosts(@Payload String post) throws IOException, InterruptedException {

        // Temporally hacked
        Thread.sleep(1000);

        var analysisMessage = objectMapper.readValue(post, AnalysisMessage.class);

        var originalPost = analyzedPostService.findById(analysisMessage.getPostId());

        if (originalPost.isEmpty()) {
            log.error("Post " + analysisMessage.getPostId() + " not found.");
            return;
        }

        var componentAnalysis = objectMapper.createObjectNode().set(analysisMessage.getComponentId(),
                analysisMessage.getAnalysis());

        originalPost.get().getAnalyses().add(componentAnalysis.toString());

        analyzedPostService.update(originalPost.get());
    }
}