package cz.cuni.mff.swproject.services;

import cz.cuni.mff.swproject.model.Post;
import cz.cuni.mff.swproject.repositories.PostRepository;
import lombok.AllArgsConstructor;
import lombok.NonNull;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Service
@AllArgsConstructor
public class PostServiceImpl implements PostService {

    private PostRepository repository;

    @Override
    public void create(@NonNull Post post) {
        repository.insert(post);
    }

    @Override
    public void create(@NonNull List<Post> posts) {
        repository.insert(posts);
    }

    @Override
    public Post findById(@NonNull UUID id) {
        return repository.findById(id);
    }

    @Override
    public List<Post> findById(@NonNull List<UUID> ids) {
        return repository.findById(ids);
    }

    @Override
    public List<Post> searchByCollection(@NonNull String collection, @NonNull Pageable pageable) {
        return repository.findByCollection(collection, pageable);
    }

    @Override
    public List<Post> searchByText(@NonNull String text, @NonNull String collection, @NonNull Pageable pageable) {
        return repository.findByTextAndCollection(text, collection, pageable);
    }

    @Override
    public List<Post> searchByKeyword(@NonNull String keyword, @NonNull String collection, @NonNull Pageable pageable) {
        return repository.findByKeywordsContainingAndCollection(keyword, collection, pageable);
    }
}
