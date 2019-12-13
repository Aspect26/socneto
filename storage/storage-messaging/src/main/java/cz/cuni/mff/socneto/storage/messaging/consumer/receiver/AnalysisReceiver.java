package cz.cuni.mff.socneto.storage.messaging.consumer.receiver;

import com.fasterxml.jackson.databind.ObjectMapper;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchAnalysisDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchAnalysisResultDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchAnalysisDtoService;
import cz.cuni.mff.socneto.storage.messaging.consumer.model.AnalysisMessage;
import cz.cuni.mff.socneto.storage.messaging.consumer.model.AnalysisMessage.AnalysisMessageResult;
import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.util.Map;
import java.util.stream.Collectors;

@Slf4j
@Service
@AllArgsConstructor
public class AnalysisReceiver {

    private final SearchAnalysisDtoService searchAnalysisDtoService;
//    private final AnalyzedPostDtoService searchAnalysisDtoService;

    private final ObjectMapper objectMapper = new ObjectMapper();

    @KafkaListener(topics = "${app.topic.toDbAnalyzed}")
    public void listenAnalyzedPosts(@Payload String post) throws IOException {

        var analysisMessage = objectMapper.readValue(post, AnalysisMessage.class);
        var searchAnalysisDto = map(analysisMessage);

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
