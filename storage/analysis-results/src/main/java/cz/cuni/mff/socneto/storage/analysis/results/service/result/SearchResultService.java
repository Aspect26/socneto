package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import cz.cuni.mff.socneto.storage.analysis.results.repository.SearchPostRepository;
import lombok.RequiredArgsConstructor;
import org.elasticsearch.index.query.BoolQueryBuilder;
import org.elasticsearch.index.query.QueryBuilders;
import org.elasticsearch.search.SearchHit;
import org.springframework.data.elasticsearch.core.ElasticsearchOperations;
import org.springframework.data.elasticsearch.core.query.NativeSearchQueryBuilder;
import org.springframework.stereotype.Component;

import java.time.Instant;
import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.stream.Collectors;

@Component
@RequiredArgsConstructor
public class SearchResultService {
    public static final String JOB_ID_FIELD = "jobId";
    private static final String RESULTS_FIELD = "results";
    private static final String COMPONENT_ID_FIELD = "componentId";

    private final ElasticsearchOperations elasticsearchOperations;

    @SuppressWarnings("unchecked")
    public List<Object> queryList(UUID jobId, String componentId, String resultName, String valueName) {
        NativeSearchQueryBuilder query = createSearchQueryBuilder(jobId, componentId);

        return elasticsearchOperations.query(query.build(),
                searchResponse -> Arrays.stream(searchResponse.getHits().getHits())
                        .map(SearchHit::getSourceAsMap)
                        .map(map -> map.get(RESULTS_FIELD))
                        .map(inner -> (Map<String, Object>) inner)
                        .map(map -> map.get(resultName))
                        .map(inner -> (Map<String, Object>) inner)
                        .map(map -> map.get(valueName))
                        .collect(Collectors.toList()));
    }

    @SuppressWarnings("unchecked")
    public List<List<Object>> queryListPair(
            UUID jobId, String componentId, String resultName1, String valueName1, String resultName2, String valueName2
    ) {
        NativeSearchQueryBuilder query = createSearchQueryBuilder(jobId, componentId);

        return elasticsearchOperations.query(query.build(),
                searchResponse -> Arrays.stream(searchResponse.getHits().getHits())
                        .map(SearchHit::getSourceAsMap)
                        .map(map -> map.get(RESULTS_FIELD))
                        .map(inner -> (Map<String, Object>) inner)
                        .map(map -> List.of(
                                ((Map<String, Object>) map.get(resultName1)).get(valueName1),
                                ((Map<String, Object>) map.get(resultName2)).get(valueName2)
                                )
                        )
                        .collect(Collectors.toList()));

    }

    public List<List<Object>> queryListWithTime(UUID jobId, String componentId, String resultName, String valueName) {
        NativeSearchQueryBuilder query = createSearchQueryBuilder(jobId, componentId);

        return elasticsearchOperations.query(query.build(),
                searchResponse -> Arrays.stream(searchResponse.getHits().getHits())
                        .map(SearchHit::getSourceAsMap)
                        .map(map -> List.of(
                                Date.from(Instant.ofEpochMilli((Long)map.get("datetime"))),
                                getNode(getNode(map, RESULTS_FIELD), resultName).get(valueName
                                )))
                        .collect(Collectors.toList()));
    }

    @SuppressWarnings("unchecked")
    private Map<String, Object> getNode(Map<String, Object> map, String name) {
        return (Map<String, Object>) map.get(name);
    }

    private NativeSearchQueryBuilder createSearchQueryBuilder(UUID jobId, String componentId) {
        BoolQueryBuilder filter = QueryBuilders.boolQuery()
                .filter(QueryBuilders.termQuery(COMPONENT_ID_FIELD, componentId))
                .filter(QueryBuilders.termQuery(JOB_ID_FIELD, jobId.toString()));
        return new NativeSearchQueryBuilder().withQuery(filter).withIndices("analyses");
    }

}
