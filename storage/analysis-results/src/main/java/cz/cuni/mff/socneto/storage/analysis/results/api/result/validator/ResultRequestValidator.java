package cz.cuni.mff.socneto.storage.analysis.results.api.result.validator;

import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.AggregationResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ListParamsResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ListResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.SingleResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.service.result.ResultRequestDtoVisitor;
import org.springframework.stereotype.Component;

import java.util.Collection;
import java.util.List;

@Component
public class ResultRequestValidator implements ResultRequestDtoVisitor<Void> {

    @Override
    public Void requestResults(SingleResultRequest resultRequest) {
        validateBase(resultRequest);
        notNullOrEmpty("valueName", resultRequest.getValueName());
        notNullOrEmpty("resultName", resultRequest.getResultName());
        return null;
    }

    @Override
    public Void requestResults(ListResultRequest resultRequest) {
        validateBase(resultRequest);
        notNullOrEmpty("params", resultRequest.getParams());
        validateParams(resultRequest.getParams());
        return null;
    }

    @Override
    public Void requestResults(AggregationResultRequest resultRequest) {
        notNullOrEmpty("jobId", resultRequest.getJobId());
        validateParams(resultRequest.getParams());
        return null;
    }

    private void validateParams(List<ListParamsResultRequest> params) {
        if (params != null) {
            params.forEach(r -> {
                notNullOrEmpty("valueName", r.getValueName());
                notNullOrEmpty("resultName", r.getResultName());
            });
        }
    }

    private Void validateBase(ResultRequest resultRequest) {
        notNullOrEmpty("componentId", resultRequest.getComponentId());
        notNullOrEmpty("jobId", resultRequest.getJobId());
        notNullOrEmpty("resultRequestType", resultRequest.getResultRequestType());
        return null;
    }

    private void notNullOrEmpty(String valueName, Object value) {
        if (value == null) {
            throw new IllegalArgumentException(valueName + " can't be null!");
        }

        if (value instanceof String && ((String) value).isBlank()) {
            throw new IllegalArgumentException(valueName + " can't be empty!");
        }

        if (value instanceof Collection && ((Collection) value).isEmpty()) {
            throw new IllegalArgumentException(valueName + " can't be empty!");
        }
    }
}
