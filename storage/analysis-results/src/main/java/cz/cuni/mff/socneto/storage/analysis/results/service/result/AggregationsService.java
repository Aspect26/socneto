package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import lombok.Data;
import lombok.RequiredArgsConstructor;
import org.elasticsearch.index.query.BoolQueryBuilder;
import org.elasticsearch.index.query.QueryBuilders;
import org.elasticsearch.script.Script;
import org.elasticsearch.script.ScriptType;
import org.elasticsearch.search.aggregations.AggregationBuilders;
import org.elasticsearch.search.aggregations.metrics.scripted.InternalScriptedMetric;
import org.elasticsearch.search.aggregations.metrics.scripted.ScriptedMetricAggregationBuilder;
import org.springframework.data.elasticsearch.core.ElasticsearchOperations;
import org.springframework.data.elasticsearch.core.query.NativeSearchQueryBuilder;
import org.springframework.stereotype.Component;

import java.util.Collections;
import java.util.Map;
import java.util.UUID;

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
        var filter = createFilterQuery(jobId, componentId); // comp_2

        return (Map<String, Double>) executeQuery(aggregation, filter).aggregation();
    }

    @SuppressWarnings("unchecked")
    public Map<String, Integer> listCount(UUID jobId, String componentId, String resultName, String valueName) {

        var aggregation = createAggregation(resultName, valueName, COUNT_INIT_SCRIPT, COUNT_MAP_SCRIPT, COUNT_COMBINE_SCRIPT, COUNT_REDUCE_SCRIPT);
        var filter = createFilterQuery(jobId, componentId); // comp_3

        return (Map<String, Integer>) executeQuery(aggregation, filter).aggregation();

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
        // TODO: jobId
        return QueryBuilders.boolQuery().filter(QueryBuilders.termQuery("componentName", componentId)); // TODO componentId
    }

    private InternalScriptedMetric executeQuery(ScriptedMetricAggregationBuilder aggregation, BoolQueryBuilder filter) {
        var query = new NativeSearchQueryBuilder().withQuery(filter).addAggregation(aggregation).build();
        return elasticsearchOperations.query(query,
                searchResponse -> searchResponse).getAggregations().get("RESULT");
    }

}
