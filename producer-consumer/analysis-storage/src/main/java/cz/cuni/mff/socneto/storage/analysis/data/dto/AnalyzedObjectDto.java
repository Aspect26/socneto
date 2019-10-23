package cz.cuni.mff.socneto.storage.analysis.data.dto;

import lombok.Builder;
import lombok.Data;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Builder
@Data
public class AnalyzedObjectDto<D, A> {
    private UUID id;
    private UUID jobId;

    private D post;

    @Builder.Default
    private List<A> analyses = new ArrayList<>();
}
