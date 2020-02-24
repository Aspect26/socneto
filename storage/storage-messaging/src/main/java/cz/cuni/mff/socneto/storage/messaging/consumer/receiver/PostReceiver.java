package cz.cuni.mff.socneto.storage.messaging.consumer.receiver;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchPostDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchPostDtoService;
import cz.cuni.mff.socneto.storage.messaging.consumer.model.PostMessage;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Service;

import java.io.IOException;

@Slf4j
@Service
public class PostReceiver {

    private final SearchPostDtoService searchPostDtoService;

    private final ObjectMapper objectMapper;

    public PostReceiver(SearchPostDtoService searchPostDtoService) {
        this.searchPostDtoService = searchPostDtoService;
        objectMapper = new ObjectMapper().configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
    }

    @KafkaListener(topics = "${app.topic.toDbRaw}")
    public void listenPosts(@Payload String post) throws IOException {

        PostMessage postMessage = objectMapper.readValue(post, PostMessage.class);

        var searchPost = new SearchPostDto();
        searchPost.setId(postMessage.getPostId());
        searchPost.setJobId(postMessage.getJobId());
        searchPost.setOriginalId(postMessage.getOriginalPostId());
        searchPost.setText(postMessage.getText());
        searchPost.setOriginalText(postMessage.getOriginalText());
        searchPost.setAuthorId(postMessage.getAuthorId());
        searchPost.setLanguage(postMessage.getLanguage());
        searchPost.setDatetime(postMessage.getDateTime());

        searchPostDtoService.create(searchPost);
    }
}
