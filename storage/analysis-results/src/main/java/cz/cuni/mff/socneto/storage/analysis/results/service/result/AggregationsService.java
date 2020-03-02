package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import lombok.RequiredArgsConstructor;
import org.elasticsearch.index.query.BoolQueryBuilder;
import org.elasticsearch.index.query.QueryBuilders;
import org.elasticsearch.script.Script;
import org.elasticsearch.script.ScriptType;
import org.elasticsearch.search.aggregations.AggregationBuilders;
import org.elasticsearch.search.aggregations.BucketOrder;
import org.elasticsearch.search.aggregations.bucket.histogram.DateHistogramInterval;
import org.elasticsearch.search.aggregations.bucket.histogram.InternalDateHistogram;
import org.elasticsearch.search.aggregations.metrics.scripted.InternalScriptedMetric;
import org.elasticsearch.search.aggregations.metrics.scripted.ScriptedMetricAggregationBuilder;
import org.joda.time.DateTime;
import org.springframework.data.elasticsearch.core.ElasticsearchOperations;
import org.springframework.data.elasticsearch.core.query.NativeSearchQueryBuilder;
import org.springframework.stereotype.Component;

import java.time.Instant;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.UUID;
import java.util.stream.Collectors;

@Component
@RequiredArgsConstructor
public class AggregationsService {

    private static final String SUM_INIT_SCRIPT = "agg_map_init";
    private static final String SUM_MAP_SCRIPT = "agg_map_map";
    private static final String SUM_COMBINE_SCRIPT = "agg_map_combine";
    private static final String SUM_REDUCE_SCRIPT = "agg_map_reduce_sum";

    private static final String COUNT_INIT_SCRIPT = "agg_list_map_init";
    private static final String COUNT_MAP_SCRIPT = "agg_list_map_map";
    private static final String COUNT_COMBINE_SCRIPT = "agg_list_map_combine";
    private static final String COUNT_REDUCE_SCRIPT = "agg_list_map_reduce_sum";

    private final ElasticsearchOperations elasticsearchOperations;

    @SuppressWarnings("unchecked")
    public Map<String, Double> mapSum(UUID jobId, String componentId, String resultName, String valueName) {

        var aggregation = createAggregation(resultName, valueName, SUM_INIT_SCRIPT, SUM_MAP_SCRIPT, SUM_COMBINE_SCRIPT, SUM_REDUCE_SCRIPT);
        var filter = createFilterQuery(jobId, componentId);

        var result = (Map<String, Double>) executeQuery(aggregation, filter).aggregation();

        return result.entrySet().stream()
                .sorted(Comparator.comparingDouble(e -> e.getValue() * -1))
                .filter(e -> e.getKey().length() > 2)
                .limit(200)
                .collect(Collectors.toMap(Map.Entry::getKey, Map.Entry::getValue, (o, n) -> n, LinkedHashMap::new));
    }

    @SuppressWarnings("unchecked")
    public Map<String, Integer> listCount(UUID jobId, String componentId, String resultName, String valueName) {

        var aggregation = createAggregation(resultName, valueName, COUNT_INIT_SCRIPT, COUNT_MAP_SCRIPT, COUNT_COMBINE_SCRIPT, COUNT_REDUCE_SCRIPT);
        var filter = createFilterQuery(jobId, componentId);

        var result = (Map<String, Integer>) executeQuery(aggregation, filter).aggregation();

        return result.entrySet().stream()
                .filter(e -> e.getKey().length() > 2)
                .sorted(Comparator.comparingInt(e -> e.getValue() * -1))
                .limit(200)
                .collect(Collectors.toMap(Map.Entry::getKey, Map.Entry::getValue, (o, n) -> n, LinkedHashMap::new));
    }

    private ScriptedMetricAggregationBuilder createAggregation(
            String resultName,
            String valueName,
            String initScriptName,
            String mapScriptName,
            String combineScriptName,
            String reduceScriptName
    ) {
        return AggregationBuilders
                .scriptedMetric("RESULT")
                .initScript(new Script(ScriptType.STORED, null, initScriptName, Collections.emptyMap()))
                .mapScript(
                        new Script(
                                ScriptType.STORED,
                                null, mapScriptName,
                                Map.of("resultName", resultName,
                                        "valueName", valueName))
                )
                .combineScript(new Script(ScriptType.STORED, null, combineScriptName, Collections.emptyMap()))
                .reduceScript(new Script(ScriptType.STORED, null, reduceScriptName, Collections.emptyMap()));
    }

    private BoolQueryBuilder createFilterQuery(UUID jobId, String componentId) {
        return QueryBuilders.boolQuery()
                .filter(QueryBuilders.termQuery("componentId", componentId))
                .filter(QueryBuilders.termQuery("jobId", jobId.toString()));
    }

    private InternalScriptedMetric executeQuery(ScriptedMetricAggregationBuilder aggregation, BoolQueryBuilder filter) {
        var query = new NativeSearchQueryBuilder().withQuery(filter).addAggregation(aggregation).withIndices("analyses").build();
        return elasticsearchOperations.query(query,
                searchResponse -> searchResponse).getAggregations().get("RESULT");
    }

}
