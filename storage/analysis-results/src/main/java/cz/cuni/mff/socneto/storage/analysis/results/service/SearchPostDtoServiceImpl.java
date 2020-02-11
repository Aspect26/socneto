package cz.cuni.mff.socneto.storage.analysis.results.service;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.ListWithCount;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchPostDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchPostDtoService;
import cz.cuni.mff.socneto.storage.analysis.results.data.mapper.SearchPostMapper;
import lombok.RequiredArgsConstructor;
import org.elasticsearch.index.query.BoolQueryBuilder;
import org.elasticsearch.index.query.QueryBuilders;
import org.elasticsearch.search.SearchHit;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.elasticsearch.core.ElasticsearchOperations;
import org.springframework.data.elasticsearch.core.query.NativeSearchQueryBuilder;
import org.springframework.stereotype.Service;

import java.time.Instant;
import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class SearchPostDtoServiceImpl implements SearchPostDtoService {

    private static final String TEXT_FIELD = "text";

    private final SearchPostService searchPostService;
    private final SearchPostMapper postMapper;

    private final ElasticsearchOperations elasticsearchOperations;

    @Override
    public SearchPostDto create(SearchPostDto searchPostDto) {
        return postMapper.searchPostToSearchPostDto(searchPostService.create(postMapper.searchPostDtoToSearchPost(searchPostDto)));
    }

    @Override
    public Optional<SearchPostDto> getById(UUID id) {
        return searchPostService.getById(id).map(postMapper::searchPostToSearchPostDto);
    }

    public ListWithCount<SearchPostDto> searchPosts(UUID jobId, List<String> allowedTerms, List<String> forbiddenTerms, int page, int size) {
        BoolQueryBuilder filter = QueryBuilders.boolQuery()
                .filter(QueryBuilders.termQuery("jobId", jobId.toString()));


        allowedTerms.forEach(term -> filter.filter(QueryBuilders.termQuery(TEXT_FIELD, term.toLowerCase())));
        forbiddenTerms.forEach(term -> filter.mustNot(QueryBuilders.termQuery(TEXT_FIELD, term.toLowerCase())));

        NativeSearchQueryBuilder nativeSearchQueryBuilder = new NativeSearchQueryBuilder()
                .withQuery(filter).withPageable(PageRequest.of(page, size)).withIndices("posts");

        return elasticsearchOperations.query(nativeSearchQueryBuilder.build(),
                searchResponse -> {
                    List<SearchPostDto> result = Arrays.stream(searchResponse.getHits().getHits()).map(SearchHit::getSourceAsMap)
                            .map(hit ->
                                    SearchPostDto.builder()
                                            .id(UUID.fromString((String) hit.get("id")))
                                            .jobId(UUID.fromString((String) hit.get("jobId")))
                                            .text((String) hit.get("text"))
                                            .originalText((String) hit.get("originalText"))
                                            .datetime((Date.from(Instant.ofEpochMilli((Long) hit.get("datetime")))))
                                            .build()
                            ).collect(Collectors.toList());
                    return new ListWithCount<>(searchResponse.getHits().totalHits, result);
                });
    }
}
