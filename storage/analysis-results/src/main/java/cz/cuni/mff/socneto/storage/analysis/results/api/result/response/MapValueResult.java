package cz.cuni.mff.socneto.storage.analysis.results.api.result.response;

import lombok.Builder;
import lombok.Getter;

import java.util.Map;

@Getter
public class MapValueResult<K, V> extends AbstractResult {

    private final Map<K, V> map;

    @Builder
    public MapValueResult(String resultName, Map<K, V> map) {
        super(resultName);
        this.map = map;
    }
}
