package cz.cuni.mff.socneto.storage.analysis.results.service;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchPostDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchPostDtoService;
import cz.cuni.mff.socneto.storage.analysis.results.data.mapper.SearchPostMapper;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class SearchPostDtoServiceImpl implements SearchPostDtoService {

    private final SearchPostService searchPostService;
    private final SearchPostMapper postMapper;

    @Override
    public SearchPostDto create(SearchPostDto searchPostDto) {
        return postMapper.searchPostToSearchPostDto(searchPostService.create(postMapper.searchPostDtoToSearchPost(searchPostDto)));
    }
}
