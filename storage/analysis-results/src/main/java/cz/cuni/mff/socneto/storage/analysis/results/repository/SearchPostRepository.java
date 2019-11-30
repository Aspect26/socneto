package cz.cuni.mff.socneto.storage.analysis.results.repository;

import cz.cuni.mff.socneto.storage.analysis.results.data.model.SearchPost;
import org.springframework.data.elasticsearch.repository.ElasticsearchRepository;

import java.util.UUID;

public interface SearchPostRepository extends ElasticsearchRepository<SearchPost, UUID> {
}
