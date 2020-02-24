package cz.cuni.mff.socneto.storage.analysis.results.api.result.request;

import com.fasterxml.jackson.annotation.JsonSubTypes;
import com.fasterxml.jackson.annotation.JsonTypeInfo;
import cz.cuni.mff.socneto.storage.analysis.results.service.result.ResultRequestDtoVisitor;
import lombok.Getter;

import java.util.UUID;

@JsonTypeInfo(
        use = JsonTypeInfo.Id.NAME,
        property = "type"
)
@JsonSubTypes({
        @JsonSubTypes.Type(value = SingleResultRequest.class, name = "SINGLE"),
        @JsonSubTypes.Type(value = ListResultRequest.class, name = "LIST"),
        @JsonSubTypes.Type(value = AggregationResultRequest.class, name = "AGGREGATION"),
        @JsonSubTypes.Type(value = PostAggregationResultRequest.class, name = "POST_AGGREGATION"),
})
@Getter
public abstract class ResultRequest {
    private final ResultRequestType resultRequestType;
    private final UUID jobId;
    private final String componentId;

    ResultRequest(ResultRequestType resultRequestType, UUID jobId, String componentId) {
        this.resultRequestType = resultRequestType;
        this.jobId = jobId;
        this.componentId = componentId;
    }

    public abstract <T> T visit(ResultRequestDtoVisitor<T> resultRequestDtoVisitor);
}
