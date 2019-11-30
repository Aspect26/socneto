package cz.cuni.mff.socneto.storage.analysis.results.service;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchAnalysisDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchAnalysisDtoService;
import cz.cuni.mff.socneto.storage.analysis.results.data.mapper.SearchAnalysisMapper;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class SearchAnalysisDtoServiceImpl implements SearchAnalysisDtoService {

    private final SearchAnalysisService searchAnalysisService;
    private final SearchAnalysisMapper searchAnalysisMapper;

    @Override
    public SearchAnalysisDto create(SearchAnalysisDto searchAnalysisDto) {
        return searchAnalysisMapper.searchAnalysisToSearchAnalysisDto(searchAnalysisService.create(searchAnalysisMapper.searchAnalysisDtoToSearchAnalysis(searchAnalysisDto)));
    }
}
