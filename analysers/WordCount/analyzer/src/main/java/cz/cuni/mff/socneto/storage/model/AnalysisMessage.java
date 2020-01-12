package cz.cuni.mff.socneto.storage.model;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;

import java.util.List;
import java.util.Map;
import java.util.UUID;

@Data
@Builder
public class AnalysisMessage {
    private UUID postId;
    private UUID jobId;
    private String componentId;
    private Map<String, AnalysisResult> results;

    @Data
    @Builder
    public static class AnalysisResult {
        private Double numberValue;
        private String textValue;
        private List<Double> numberListValue;
        private List<String> textListValue;
        private Map<String, Double> numberMapValue;
        private Map<String, String> textMapValue;
    }
}
