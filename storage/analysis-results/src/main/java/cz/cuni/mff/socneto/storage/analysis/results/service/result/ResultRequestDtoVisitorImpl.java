package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.AggregationResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ListResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.SingleResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.ListValueResult;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.MapValueResult;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.Result;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class ResultRequestDtoVisitorImpl implements ResultRequestDtoVisitor {

    private final AggregationsService aggregationsService;
    private final SearchService searchService;

    @Override
    public Result requestResults(SingleResultRequest resultRequest) {
        return null;
    }

    @Override
    public Result requestResults(ListResultRequest resultRequest) {
        // TODO
//                var list = searchService.queryList(resultRequest.getJobId(), resultRequest.getComponentName(), resultRequest.getResultName(), resultRequest.getValueName());
        switch (resultRequest.getResultRequestType()) {
            case LIST:
                var list = searchService.queryListPair(resultRequest.getJobId(), resultRequest.getComponentId(),
                        resultRequest.getParams().get(0).getResultName(), resultRequest.getParams().get(0).getValueName(),
                        resultRequest.getParams().get(1).getResultName(), resultRequest.getParams().get(1).getValueName());
                return new ListValueResult<>("list", list);
            case LIST_WITH_TIME:
                var listWithTime = searchService.queryListWithTime(null,resultRequest.getComponentId(), resultRequest.getParams().get(0).getResultName(), resultRequest.getParams().get(0).getValueName());
                return new ListValueResult<>("listPairWithTime", listWithTime);
        }

        throw new IllegalArgumentException("error");
    }

    @Override
    public Result requestResults(AggregationResultRequest resultRequest) {
        switch (resultRequest.getResultRequestType()) {
            case MAP_SUM:
                var sums = aggregationsService.mapSum(resultRequest.getJobId(), resultRequest.getComponentId(), resultRequest.getResultName(), resultRequest.getValueName());
                return MapValueResult.<String, Double>builder().resultName(resultRequest.getResultName()).map(sums).build();
            case LIST_COUNT:
                var counts = aggregationsService.listCount(null, resultRequest.getComponentId(), resultRequest.getResultName(), resultRequest.getValueName());
                return MapValueResult.<String, Integer>builder().resultName(resultRequest.getResultName()).map(counts).build();
        }

        throw new IllegalArgumentException("error");
    }
}
