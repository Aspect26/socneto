package cz.cuni.mff.socneto.storage.analysis.results.api.result.response;

import lombok.Getter;

@Getter
public class SingleValueResult<T> extends AbstractResult {

    private final T value;

    public SingleValueResult(String resultName, T value) {
        super(resultName);
        this.value = value;
    }
}
