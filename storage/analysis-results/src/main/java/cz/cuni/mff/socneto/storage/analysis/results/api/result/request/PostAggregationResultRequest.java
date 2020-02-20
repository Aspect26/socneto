package cz.cuni.mff.socneto.storage.analysis.results.api.result.request;

import com.fasterxml.jackson.annotation.JsonTypeName;
import cz.cuni.mff.socneto.storage.analysis.results.service.result.ResultRequestDtoVisitor;
import lombok.Getter;

import java.util.UUID;

@Getter
@JsonTypeName("POST_AGGREGATION")
public class PostAggregationResultRequest extends ResultRequest {

    public PostAggregationResultRequest(ResultRequestType resultRequestType, UUID jobId, String componentId) {
        super(resultRequestType, jobId, componentId);
    }

    @Override
    public <T> T visit(ResultRequestDtoVisitor<T> resultRequestDtoVisitor) {
        return resultRequestDtoVisitor.requestResults(this);
    }
}
