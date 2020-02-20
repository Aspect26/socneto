package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.AggregationResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ListResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.PostAggregationResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.SingleResultRequest;

public interface ResultRequestDtoVisitor<T> {

    T requestResults(SingleResultRequest resultRequest);

    T requestResults(ListResultRequest resultRequest);

    T requestResults(AggregationResultRequest resultRequest);

    T requestResults(PostAggregationResultRequest resultRequest);

}
