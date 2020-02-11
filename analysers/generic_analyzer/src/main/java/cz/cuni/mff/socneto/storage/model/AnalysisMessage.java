package cz.cuni.mff.socneto.storage.model;

import lombok.Builder;
import lombok.Data;

import java.util.List;
import java.util.Map;
import java.util.UUID;

/**
 * Analysis result message, see Analyzer documentation.
 */
@Data
@Builder
public class AnalysisMessage {
    private UUID postId;
    private UUID jobId;
    private String componentId;
    private Map<String, AnalysisResult> results;

    /**
     * Only one value should be specified otherwise there is unexpected behaviour.
     * This value is specified in output format.
     */
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
