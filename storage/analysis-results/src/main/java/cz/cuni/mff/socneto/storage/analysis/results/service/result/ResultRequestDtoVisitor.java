package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.AggregationResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ListResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.SingleResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.Result;

public interface ResultRequestDtoVisitor {

    Result requestResults(SingleResultRequest resultRequest);

    Result requestResults(ListResultRequest resultRequest);

    Result requestResults(AggregationResultRequest resultRequest);

}
