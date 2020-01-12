package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import lombok.RequiredArgsConstructor;
import lombok.Value;
import org.elasticsearch.index.query.BoolQueryBuilder;
import org.elasticsearch.index.query.QueryBuilders;
import org.elasticsearch.search.SearchHit;
import org.springframework.data.elasticsearch.core.ElasticsearchOperations;
import org.springframework.data.elasticsearch.core.query.NativeSearchQueryBuilder;
import org.springframework.stereotype.Component;

import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.stream.Collectors;

@Component
@RequiredArgsConstructor
public class SearchService {

    private final ElasticsearchOperations elasticsearchOperations;

    @SuppressWarnings("unchecked")
    public List<Object> queryList(UUID jobId, String componentId, String resultName, String valueName) {

        BoolQueryBuilder filter = QueryBuilders.boolQuery().filter(QueryBuilders.termQuery("componentId", componentId));
        var query = new NativeSearchQueryBuilder().withQuery(filter);

        return elasticsearchOperations.query(query.build(),
                searchResponse -> Arrays.stream(searchResponse.getHits().getHits())
                        .map(SearchHit::getSourceAsMap)
                        .map(map -> map.get("results"))
                        .map(inner -> (Map<String, Object>) inner)
                        .map(map -> map.get(resultName))
                        .map(inner -> (Map<String, Object>) inner)
                        .map(map -> map.get(valueName))
                        .collect(Collectors.toList()));

    }

    @SuppressWarnings("unchecked")
    public List<List<Object>> queryListPair(UUID jobId, String componentId, String resultName1,
                                                    String valueName1, String resultName2,
                                                    String valueName2) {

        BoolQueryBuilder filter = QueryBuilders.boolQuery().filter(QueryBuilders.termQuery("componentId", componentId));
        var query = new NativeSearchQueryBuilder().withQuery(filter);

        return elasticsearchOperations.query(query.build(),
                searchResponse -> Arrays.stream(searchResponse.getHits().getHits())
                        .map(SearchHit::getSourceAsMap)
                        .map(map -> map.get("results"))
                        .map(inner -> (Map<String, Object>) inner)
                        .map(map -> List.of(
                                        ((Map<String, Object>) map.get(resultName1)).get(valueName1),
                                        ((Map<String, Object>) map.get(resultName2)).get(valueName2)
                                )
                        )
                        .collect(Collectors.toList()));

    }

    @SuppressWarnings("unchecked")
    public List<List<Object>> queryListWithTime(UUID jobId, String componentId, String resultName,
                                                     String valueName) {

        BoolQueryBuilder filter = QueryBuilders.boolQuery().filter(QueryBuilders.termQuery("componentId", componentId));
        var query = new NativeSearchQueryBuilder().withQuery(filter);

        return elasticsearchOperations.query(query.build(),
                searchResponse -> Arrays.stream(searchResponse.getHits().getHits())
                        .map(SearchHit::getSourceAsMap)
                        .map(map -> map.get("results"))
                        .map(inner -> (Map<String, Object>) inner)
                        .map(map -> {
                            try {

                                Thread.sleep(100);
                            } catch (InterruptedException e) {

                            }
                            return List.of(
                                    System.currentTimeMillis(),
                                    ((Map<String, Object>) map.get(resultName)).get(valueName));
                        })

                        .collect(Collectors.toList()));

    }

}
