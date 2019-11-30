package cz.cuni.mff.socneto.storage.analysis.results.data.mapper;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchAnalysisDto;
import cz.cuni.mff.socneto.storage.analysis.results.data.model.SearchAnalysis;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR, implementationName = "mp")
// TODO remove implementationName
public interface SearchAnalysisMapper {

    SearchAnalysis searchAnalysisDtoToSearchAnalysis(SearchAnalysisDto searchAnalysisDto);

    SearchAnalysisDto searchAnalysisToSearchAnalysisDto(SearchAnalysis searchAnalysis);
}