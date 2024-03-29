package cz.cuni.mff.socneto.storage.analysis.results.service;

import cz.cuni.mff.socneto.storage.analysis.results.data.model.SearchAnalysis;
import cz.cuni.mff.socneto.storage.analysis.results.repository.SearchAnalysisRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class SearchAnalysisService {

    private final SearchAnalysisRepository searchAnalysisRepository;

    public SearchAnalysis create(SearchAnalysis searchAnalysis) {
        searchAnalysis.setId(
                (long) searchAnalysis.getPostId().hashCode() *
                (long) searchAnalysis.getComponentId().hashCode() *
                (long) searchAnalysis.getJobId().hashCode()
        );
        return searchAnalysisRepository.save(searchAnalysis);
    }

}
