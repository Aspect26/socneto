package cz.cuni.mff.socneto.storage.analysis.results.service;

import cz.cuni.mff.socneto.storage.analysis.results.data.model.SearchPost;
import cz.cuni.mff.socneto.storage.analysis.results.repository.SearchPostRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.Optional;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class SearchPostService {

    private final SearchPostRepository searchPostRepository;

    public SearchPost create(SearchPost searchPost) {
        return searchPostRepository.save(searchPost);
    }

    public Optional<SearchPost> getById(UUID id) {
        return searchPostRepository.findById(id);
    }

}
