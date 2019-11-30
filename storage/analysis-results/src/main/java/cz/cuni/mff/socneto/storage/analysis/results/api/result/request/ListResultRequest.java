package cz.cuni.mff.socneto.storage.analysis.results.api.result.request;

import com.fasterxml.jackson.annotation.JsonTypeName;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.Result;
import cz.cuni.mff.socneto.storage.analysis.results.service.result.ResultRequestDtoVisitor;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Value;

import java.util.List;
import java.util.UUID;

@Getter
@JsonTypeName("LIST")
public class ListResultRequest extends ResultRequest {

    private final List<ListParamsResultRequest> params;

    public ListResultRequest(ResultRequestType resultRequestType, UUID jobId, String componentId, List<ListParamsResultRequest> params) {
        super(resultRequestType, jobId, componentId);
        this.params = params;
    }

    @Override
    public Result visit(ResultRequestDtoVisitor resultRequestDtoVisitor) {
        return resultRequestDtoVisitor.requestResults(this);
    }

    @Value
    @AllArgsConstructor
    public static class ListParamsResultRequest {
        private final String resultName;
        private final String valueName;
    }
}
