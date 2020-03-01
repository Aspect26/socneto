package cz.cuni.mff.socneto.storage.analysis.results.repository;

import cz.cuni.mff.socneto.storage.analysis.results.data.model.SearchAnalysis;
import org.springframework.data.elasticsearch.repository.ElasticsearchRepository;

public interface SearchAnalysisRepository extends ElasticsearchRepository<SearchAnalysis, Long> {

}
