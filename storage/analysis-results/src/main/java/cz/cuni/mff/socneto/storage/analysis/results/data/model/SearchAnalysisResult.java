package cz.cuni.mff.socneto.storage.analysis.results.data.model;

import lombok.Data;

import java.util.List;
import java.util.Map;

@Data
public class SearchAnalysisResult {
    private Double numberValue;
    private String textValue;
    private List<Double> numberListValue;
    private List<String> textListValue;
    private Map<String, Double> numberMapValue;
    private Map<String, String> textMapValue;
}