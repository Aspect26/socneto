package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.AggregationResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ListResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.PostAggregationResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.SingleResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.ListValueResult;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.MapValueResult;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.Result;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.Date;
import java.util.List;

@Service
@RequiredArgsConstructor
public class ResultRequestDtoProcessorVisitor implements ResultRequestDtoVisitor<Result> {

    private final AggregationsService aggregationsService;
    private final SearchResultService searchResultService;
    private final PostAggregationsService postAggregationsService;

    @Override
    public Result requestResults(SingleResultRequest resultRequest) {
        return null;
    }

    @Override
    public Result requestResults(ListResultRequest resultRequest) {
        switch (resultRequest.getResultRequestType()) {
            case LIST:
                if (resultRequest.getParams().size() == 1) {
                    var list = searchResultService.queryList(resultRequest.getJobId(), resultRequest.getComponentId(),
                            resultRequest.getParams().get(0).getResultName(), resultRequest.getParams().get(0).getValueName(),
                            resultRequest.getPage(), resultRequest.getSize());
                    return new ListValueResult<>("LIST", List.of(list.getList()), list.getTotalCount());
                } else if (resultRequest.getParams().size() == 2) {
                    var listPair = searchResultService.queryListPair(resultRequest.getJobId(), resultRequest.getComponentId(),
                            resultRequest.getParams().get(0).getResultName(), resultRequest.getParams().get(0).getValueName(),
                            resultRequest.getParams().get(1).getResultName(), resultRequest.getParams().get(1).getValueName(),
                            resultRequest.getPage(), resultRequest.getSize());
                    return new ListValueResult<>("LIST", listPair.getList(), listPair.getTotalCount());
                } else {
                    throw new IllegalArgumentException("too many arguments");
                }
            case LIST_WITH_TIME:
                var listWithTime = searchResultService
                        .queryListWithTime(resultRequest.getJobId(), resultRequest.getComponentId(), resultRequest.getParams().get(0).getResultName(),
                                resultRequest.getParams().get(0).getValueName(), resultRequest.getPage(), resultRequest.getSize());
                return new ListValueResult<>("LIST_WITH_TIME", listWithTime.getList(), listWithTime.getTotalCount());
        }

        throw new IllegalArgumentException("Not supported operation");
    }

    @Override
    public Result requestResults(AggregationResultRequest resultRequest) {
        if (resultRequest.getParams().size() != 1) {
            throw new IllegalArgumentException("Wrong size of arguments.");
        }

        var params = resultRequest.getParams().get(0);

        switch (resultRequest.getResultRequestType()) {
            case MAP_SUM:
                var sums = aggregationsService.mapSum(resultRequest.getJobId(), resultRequest.getComponentId(),
                        params.getResultName(), params.getValueName());
                return MapValueResult.<String, Double>builder().resultName(params.getResultName()).map(sums).build();
            case LIST_COUNT:
                var counts = aggregationsService.listCount(resultRequest.getJobId(), resultRequest.getComponentId(),
                        params.getResultName(), params.getValueName());
                return MapValueResult.<String, Integer>builder().resultName(params.getResultName()).map(counts).build();
            default:
                throw new IllegalArgumentException("Not supported operation");
        }
    }

    @Override
    public Result requestResults(PostAggregationResultRequest resultRequest) {
        switch (resultRequest.getResultRequestType()) {
            case COUNT_PER_TIME:
                var timeCounts = postAggregationsService.coutInTime(resultRequest.getJobId());
                return MapValueResult.<Date, Long>builder().resultName("count_per_time").map(timeCounts).build();
            case COUNT_PER_AUTHOR:
                var authorCounts = postAggregationsService.coutPerAuthor(resultRequest.getJobId());
                return MapValueResult.<String, Long>builder().resultName("count_per_author").map(authorCounts).build();
            case COUNT_PER_LANGUAGE:
                var languageCounts = postAggregationsService.coutPerLanguage(resultRequest.getJobId());
                return MapValueResult.<String, Long>builder().resultName("count_per_language").map(languageCounts).build();
            default:
                throw new IllegalArgumentException("Not supported operation");
        }
    }
}
