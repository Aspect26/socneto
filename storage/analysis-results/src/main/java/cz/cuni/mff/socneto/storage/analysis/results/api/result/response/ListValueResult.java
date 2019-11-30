package cz.cuni.mff.socneto.storage.analysis.results.api.result.response;

import lombok.Builder;
import lombok.Getter;

import java.util.List;

@Getter
public class ListValueResult<T> extends AbstractResult {

    private final List<List<T>> list;

    @Builder
    public ListValueResult(String resultName, List<List<T>> list) {
        super(resultName);
        this.list = list;
    }
}
