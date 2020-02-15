package cz.cuni.mff.socneto.storage.messaging.consumer.receiver;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchAnalysisDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchAnalysisResultDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchPostDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchAnalysisDtoService;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchPostDtoService;
import cz.cuni.mff.socneto.storage.messaging.consumer.model.AnalysisMessage;
import cz.cuni.mff.socneto.storage.messaging.consumer.model.AnalysisMessage.AnalysisMessageResult;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;

@Slf4j
@Service
public class AnalysisReceiver {

    private final SearchAnalysisDtoService searchAnalysisDtoService;
    private final SearchPostDtoService searchPostDtoService;

    private final ObjectMapper objectMapper;

    public AnalysisReceiver(SearchAnalysisDtoService searchAnalysisDtoService, SearchPostDtoService searchPostDtoService) {
        this.searchAnalysisDtoService = searchAnalysisDtoService;
        this.searchPostDtoService = searchPostDtoService;
        objectMapper = new ObjectMapper();
        objectMapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
    }

    @KafkaListener(topics = "${app.topic.toDbAnalyzed}")
    public void listenAnalyzedPosts(@Payload String analysis) throws IOException {

        var analysisMessage = objectMapper.readValue(analysis, AnalysisMessage.class);
        var searchAnalysisDto = map(analysisMessage);

        Optional<SearchPostDto> post;
        while ((post = searchPostDtoService.getById(searchAnalysisDto.getPostId())).isEmpty()) {
            try {
                Thread.sleep(1000);
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }
        }

        searchAnalysisDto.setDatetime(post.get().getDatetime());

        searchAnalysisDtoService.create(searchAnalysisDto);
    }

    private static SearchAnalysisDto map(AnalysisMessage analysisMessage) {
        var dto = new SearchAnalysisDto();
        dto.setPostId(analysisMessage.getPostId());
        dto.setJobId(analysisMessage.getJobId());
        dto.setComponentId(analysisMessage.getComponentId());
        dto.setResults(map(analysisMessage.getResults()));
        return dto;
    }

    private static Map<String, SearchAnalysisResultDto> map(Map<String, AnalysisMessageResult> analysisMessageResults) {
        return analysisMessageResults.entrySet().stream().collect(Collectors.toMap(
                Map.Entry::getKey,
                AnalysisReceiver::map
        ));
    }

    private static SearchAnalysisResultDto map(Map.Entry<String, AnalysisMessageResult> e) {
        var dto = new SearchAnalysisResultDto();
        dto.setNumberValue(e.getValue().getNumberValue());
        dto.setTextValue(e.getValue().getTextValue());
        dto.setNumberListValue(e.getValue().getNumberListValue());
        dto.setTextListValue(e.getValue().getTextListValue());
        dto.setNumberMapValue(e.getValue().getNumberMapValue());
        dto.setTextMapValue(e.getValue().getTextMapValue());
        return dto;
    }
}
