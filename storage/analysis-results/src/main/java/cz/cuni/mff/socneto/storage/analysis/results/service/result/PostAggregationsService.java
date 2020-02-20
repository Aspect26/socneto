package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import lombok.RequiredArgsConstructor;
import org.elasticsearch.index.query.QueryBuilders;
import org.elasticsearch.search.aggregations.AggregationBuilders;
import org.elasticsearch.search.aggregations.BucketOrder;
import org.elasticsearch.search.aggregations.bucket.histogram.DateHistogramInterval;
import org.elasticsearch.search.aggregations.bucket.histogram.InternalDateHistogram;
import org.elasticsearch.search.aggregations.bucket.terms.InternalTerms;
import org.elasticsearch.search.aggregations.bucket.terms.StringTerms;
import org.joda.time.DateTime;
import org.springframework.data.elasticsearch.core.ElasticsearchOperations;
import org.springframework.data.elasticsearch.core.query.NativeSearchQueryBuilder;
import org.springframework.stereotype.Component;

import java.time.Instant;
import java.util.Collections;
import java.util.Date;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.UUID;
import java.util.stream.Collectors;

@Component
@RequiredArgsConstructor
public class PostAggregationsService {

    public static final String RESULT_FIELD = "RESULT";
    private final ElasticsearchOperations elasticsearchOperations;

    public Map<Date, Long> coutInTime(UUID jobId) {
        var filter = QueryBuilders.boolQuery()
                .filter(QueryBuilders.termQuery("jobId", jobId.toString()));

        var aggregation = AggregationBuilders.dateHistogram("date").dateHistogramInterval(DateHistogramInterval.HOUR).field("datetime").order(BucketOrder.key(false));

        var query = new NativeSearchQueryBuilder().withQuery(filter).addAggregation(aggregation).withIndices("posts").build();
        var result = (InternalDateHistogram) elasticsearchOperations.query(query, searchResponse -> searchResponse).getAggregations().get("date");

        return result.getBuckets().stream()
                .collect(Collectors.toMap(b -> Date.from(Instant.ofEpochMilli(((DateTime) b.getKey()).getMillis())),
                        InternalDateHistogram.Bucket::getDocCount, (o, n) -> n, LinkedHashMap::new));
    }

    public Map<String, Long> coutPerAuthor(UUID jobId) {
        return createTermAggResult(jobId, "authorId");
    }

    public Map<String, Long> coutPerLanguage(UUID jobId) {
        return createTermAggResult(jobId, "language");

    }

    private Map<String, Long> createTermAggResult(UUID jobId, String language) {
        var filter = QueryBuilders.boolQuery()
                .filter(QueryBuilders.termQuery("jobId", jobId.toString()));

        var aggregation = AggregationBuilders.terms(RESULT_FIELD).field(language).order(BucketOrder.key(false)).minDocCount(2).size(100);

        var query = new NativeSearchQueryBuilder().withQuery(filter).addAggregation(aggregation).withIndices("posts").build();
        var res = elasticsearchOperations.query(query, searchResponse -> searchResponse).getAggregations().get("RESULT");

        if (!(res instanceof StringTerms)) {
            return Collections.emptyMap();
        }

        var stringResult = (StringTerms) res;

        return stringResult.getBuckets().stream().collect(Collectors.toMap(b -> (String) b.getKey(),
                InternalTerms.Bucket::getDocCount, (o, n) -> n, LinkedHashMap::new));
    }
}
