package cz.cuni.mff.socneto.storage.analysis.results.api.result.request;

import com.fasterxml.jackson.annotation.JsonTypeName;
import cz.cuni.mff.socneto.storage.analysis.results.service.result.ResultRequestDtoVisitor;
import lombok.Getter;

import java.util.List;
import java.util.UUID;

@Getter
@JsonTypeName("LIST")
public class ListResultRequest extends ResultRequest {

    private final List<ListParamsResultRequest> params;
    private final int page;
    private final int size;

    public ListResultRequest(
            ResultRequestType resultRequestType, UUID jobId, String componentId, List<ListParamsResultRequest> params, int page, int size
    ) {
        super(resultRequestType, jobId, componentId);
        this.params = params;
        this.page = page;
        this.size = size;
    }

    @Override
    public <T> T visit(ResultRequestDtoVisitor<T> resultRequestDtoVisitor) {
        return resultRequestDtoVisitor.requestResults(this);
    }


}
