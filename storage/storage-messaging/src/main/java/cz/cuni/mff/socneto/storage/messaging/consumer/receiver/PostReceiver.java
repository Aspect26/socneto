package cz.cuni.mff.socneto.storage.messaging.consumer.receiver;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchPostDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchPostDtoService;
import cz.cuni.mff.socneto.storage.analysis.storage.api.service.AnalyzedPostDtoService;
import cz.cuni.mff.socneto.storage.messaging.consumer.model.PostMessage;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Service;

import java.io.IOException;

@Slf4j
@Service
public class PostReceiver {

    private final AnalyzedPostDtoService analyzedPostDtoService;
    private final SearchPostDtoService searchPostDtoService;

    private final ObjectMapper objectMapper;

    public PostReceiver(AnalyzedPostDtoService searchAnalysisDtoService, SearchPostDtoService searchPostDtoService) {
        this.analyzedPostDtoService = searchAnalysisDtoService;
        this.searchPostDtoService = searchPostDtoService;
        objectMapper = new ObjectMapper();
        objectMapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
    }

    @KafkaListener(topics = "${app.topic.toDbRaw}")
    public void listenPosts(@Payload String post) throws IOException {

        log.info("Post:" + post);

        PostMessage postMessage = objectMapper.readValue(post, PostMessage.class);

        // TODO internal storage
//        var postDto = PostDto.builder()
//                .id(postMessage.getPostId())
//
//                .id(postMessage.getJobId().toString())
//                .authorId(postMessage.getAuthorId())
//                .text(postMessage.getText())
//                .originalText(postMessage.getOriginalText())
//                .source(postMessage.getSource())
//                .dateTime(postMessage.getDateTime().toString())
//                .build();
//
//        var analyzedObject = AnalyzedObjectDto.<PostDto, String>builder()
//                .post(postDto)
//                .id(postMessage.getPostId())
//                .jobId(postMessage.getJobId())
//                .build();

//        analyzedPostDtoService.create(analyzedObject);

        var searchPost = new SearchPostDto();
        searchPost.setId(postMessage.getPostId());
        searchPost.setJobId(postMessage.getJobId());
        searchPost.setOriginalId(postMessage.getOriginalPostId());
        searchPost.setText(postMessage.getText());
        searchPost.setOriginalText(postMessage.getOriginalText());
        searchPost.setDatetime(postMessage.getDateTime());

        searchPostDtoService.create(searchPost);
    }
}
