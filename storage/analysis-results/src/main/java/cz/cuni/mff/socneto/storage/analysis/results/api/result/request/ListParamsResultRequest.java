package cz.cuni.mff.socneto.storage.analysis.results.api.result.request;

import lombok.AllArgsConstructor;
import lombok.Value;

@Value
@AllArgsConstructor
public class ListParamsResultRequest {
    private final String resultName;
    private final String valueName;
}