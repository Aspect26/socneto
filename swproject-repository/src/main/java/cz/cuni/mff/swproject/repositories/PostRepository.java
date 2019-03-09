package cz.cuni.mff.swproject.repositories;

import cz.cuni.mff.swproject.model.Post;
import org.springframework.data.domain.Pageable;
import org.springframework.data.mongodb.repository.MongoRepository;

import java.util.List;
import java.util.UUID;

public interface PostRepository extends MongoRepository<Post, String> {

    Post findById(UUID id);

    List<Post> findById(List<UUID> ids);

    List<Post> findByCollection(String collection, Pageable pageable);

    List<Post> findByTextAndCollection(String text, String collection, Pageable pageable);

    List<Post> findByKeywordsContainingAndCollection(String keyword, String collection, Pageable pageable);
}
