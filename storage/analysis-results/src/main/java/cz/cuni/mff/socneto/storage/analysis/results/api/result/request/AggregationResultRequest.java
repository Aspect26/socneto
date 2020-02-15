package cz.cuni.mff.socneto.storage.analysis.results.api.result.request;

import com.fasterxml.jackson.annotation.JsonTypeName;
import cz.cuni.mff.socneto.storage.analysis.results.service.result.ResultRequestDtoVisitor;
import lombok.Getter;

import java.util.List;
import java.util.UUID;

@Getter
@JsonTypeName("AGGREGATION")
public class AggregationResultRequest extends ResultRequest {

    private final List<ListParamsResultRequest> params;

    public AggregationResultRequest(ResultRequestType resultRequestType, UUID jobId, String componentId, List<ListParamsResultRequest> params) {
        super(resultRequestType, jobId, componentId);
        this.params = params;
    }

    @Override
    public <T> T visit(ResultRequestDtoVisitor<T> resultRequestDtoVisitor) {
        return resultRequestDtoVisitor.requestResults(this);
    }
}
