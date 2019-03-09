package cz.cuni.mff.swproject.controllers;

import cz.cuni.mff.swproject.model.Post;
import cz.cuni.mff.swproject.repositories.PostRepository;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Value;
import lombok.experimental.NonFinal;
import lombok.experimental.UtilityClass;
import org.springframework.data.domain.PageRequest;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.UUID;
import java.util.stream.Collectors;

@Controller
@AllArgsConstructor
public class PostController {

    private PostRepository repository;

    // TODO separate combinations
    @GetMapping("/posts/{collection}")
    @ResponseBody
    public PageableResponse<PostDto> find(@PathVariable String collection,
                                          @RequestParam(required = false) String text,
                                          @RequestParam(required = false) String keyword,
                                          @RequestParam(defaultValue = "0") int pageNumber,
                                          @RequestParam(defaultValue = "1000") int pageSize
    ) {

        String nextRequest = "/posts/" + collection + "?";
        List<Post> posts;

        if (text != null) {
            nextRequest += "text=" + text;
            posts = repository.findByTextAndCollection(text, collection, PageRequest.of(pageNumber, pageSize));
        } else if (keyword != null) {
            nextRequest += "keyword=" + keyword;
            posts = repository.findByKeywordsContainingAndCollection(keyword, collection, PageRequest.of(pageNumber, pageSize));
        } else {
            posts = repository.findByCollection(collection, PageRequest.of(pageNumber, pageSize));
        }

        nextRequest = posts.size() == pageSize ? nextRequest + "&pageNumber=" + (pageNumber + 1) + "&pageSize=" + pageSize : null;

        return PageableResponse.<PostDto>builder()
                .count(posts.size())
                .pageNumber(pageNumber)
                .pageSize(pageSize)
                .nextRequest(nextRequest)
                .data(PostMapper.toDto(posts))
                .build();
    }

    @PostMapping("/posts")
    @ResponseBody
    public List<UUID> postPosts(@RequestBody List<PostDto> postDtos) {
        List<Post> posts = postDtos.stream().map(PostMapper::fromDto).collect(Collectors.toList());
        posts.forEach(post -> post.setId(UUID.randomUUID()));
        repository.insert(posts);
        return posts.stream().map(Post::getId).collect(Collectors.toList());
    }

    @PostMapping("/post")
    @ResponseBody
    public UUID postPost(@RequestBody PostDto post) {
        return postPosts(Collections.singletonList(post)).get(0);
    }

    @GetMapping("/post/{id}/")
    @ResponseBody
    public PostDto getPost(@PathVariable String id) {
        return PostMapper.toDto(repository.findById(UUID.fromString(id)));
    }


    @Value
    @Builder
    public static class PostDto {
        @NonFinal
        private UUID id;

        private String originalId;
        private String collection;
        private String text;
        private List<String> keywords;
    }

    @Value
    @Builder
    public static class PageableResponse<D> {
        private final int count;
        private final int pageNumber;
        private final int pageSize;
        private final String nextRequest;
        private final List<D> data;
    }

    // TODO better mapper
    @UtilityClass
    public class PostMapper {

        public static Post fromDto(PostDto dto) {
            return Post.builder()
                    .collection(dto.getCollection())
                    .text(dto.getText())
                    .originalId(dto.getOriginalId())
                    .keywords(new ArrayList<>(dto.getKeywords()))
                    .build();
        }

        public static List<Post> fromDto(List<PostDto> dtos) {
            return dtos.stream().map(PostMapper::fromDto).collect(Collectors.toList());
        }

        public static PostDto toDto(Post post) {
            return PostDto.builder()
                    .id(post.getId())
                    .collection(post.getCollection())
                    .text(post.getText())
                    .originalId(post.getOriginalId())
                    .keywords(new ArrayList<>(post.getKeywords()))
                    .build();
        }

        public static List<PostDto> toDto(List<Post> posts) {
            return posts.stream().map(PostMapper::toDto).collect(Collectors.toList());
        }
    }
}
