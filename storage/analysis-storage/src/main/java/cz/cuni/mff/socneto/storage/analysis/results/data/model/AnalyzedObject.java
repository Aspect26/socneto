package cz.cuni.mff.socneto.storage.analysis.results.data.model;

import lombok.Builder;
import lombok.Data;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;
import java.util.UUID;

@Data
@Builder
@Document(collection = "analysis")
public class AnalyzedObject<D, A> {

    private UUID id;
    private UUID jobId;

    private D post;

    private List<A> analyses;

}
