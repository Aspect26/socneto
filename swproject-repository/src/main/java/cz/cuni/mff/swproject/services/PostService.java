package cz.cuni.mff.swproject.services;

import cz.cuni.mff.swproject.model.Post;
import org.springframework.data.domain.Pageable;

import java.util.List;
import java.util.UUID;

public interface PostService {

    void create(Post post);

    void create(List<Post> posts);

    Post findById(UUID id);

    List<Post> findById(List<UUID> ids);

    List<Post> searchByCollection(String collection, Pageable pageable);

    List<Post> searchByText(String text, String collection, Pageable pageable);

    List<Post> searchByKeyword(String keyword, String collection, Pageable pageable);
}
