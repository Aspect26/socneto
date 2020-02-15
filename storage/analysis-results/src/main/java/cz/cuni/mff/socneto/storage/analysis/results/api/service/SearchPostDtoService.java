package cz.cuni.mff.socneto.storage.analysis.results.api.service;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.ListWithCount;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchPostDto;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface SearchPostDtoService {

    SearchPostDto create(SearchPostDto searchPostDto);

    Optional<SearchPostDto> getById(UUID id);

    ListWithCount<SearchPostDto> searchPosts(UUID jobId, List<String> allowedTerms, List<String> forbiddenTerms, int page, int size);
}
