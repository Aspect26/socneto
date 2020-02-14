package cz.cuni.mff.socneto.storage.analysis.results.api.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Value;

import java.util.List;

@Value
@Builder
@AllArgsConstructor
public class ListWithCount<T> {
    private final long totalCount;
    private final List<T> list;
}
