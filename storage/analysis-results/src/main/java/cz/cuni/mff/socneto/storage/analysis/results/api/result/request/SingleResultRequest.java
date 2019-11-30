package cz.cuni.mff.socneto.storage.analysis.results.api.result.request;

import com.fasterxml.jackson.annotation.JsonTypeName;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.Result;
import cz.cuni.mff.socneto.storage.analysis.results.service.result.ResultRequestDtoVisitor;
import lombok.Getter;

import java.util.UUID;

@Getter
@JsonTypeName("SINGLE")
public class SingleResultRequest extends ResultRequest {

    private final String resultName;
    private final String valueName;

    public SingleResultRequest(ResultRequestType resultRequestType, UUID jobId, String componentId, String resultName1, String valueName) {
        super(resultRequestType, jobId, componentId);
        this.resultName = resultName1;
        this.valueName = valueName;
    }

    @Override
    public Result visit(ResultRequestDtoVisitor resultRequestDtoVisitor) {
        return resultRequestDtoVisitor.requestResults(this);
    }
}
