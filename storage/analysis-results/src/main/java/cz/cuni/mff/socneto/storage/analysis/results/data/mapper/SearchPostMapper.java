package cz.cuni.mff.socneto.storage.analysis.results.data.mapper;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchPostDto;
import cz.cuni.mff.socneto.storage.analysis.results.data.model.SearchPost;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface SearchPostMapper {

    SearchPost searchPostDtoToSearchPost(SearchPostDto searchPostDto);

    SearchPostDto searchPostToSearchPostDto(SearchPost post);
}