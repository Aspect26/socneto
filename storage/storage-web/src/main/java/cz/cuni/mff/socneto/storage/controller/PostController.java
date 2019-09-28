package cz.cuni.mff.socneto.storage.controller;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import cz.cuni.mff.socneto.storage.analysis.data.dto.AnalyzedObjectDto;
import cz.cuni.mff.socneto.storage.analysis.data.dto.PostDto;
import cz.cuni.mff.socneto.storage.analysis.service.AnalyzedPostService;
import lombok.Builder;
import lombok.RequiredArgsConstructor;
import lombok.Value;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.io.IOException;
import java.time.Instant;
import java.util.Date;
import java.util.List;
import java.util.UUID;
import java.util.stream.Collectors;

@RestController
@RequiredArgsConstructor
public class PostController {

    private final AnalyzedPostService analyzedPostService;

    private final ObjectMapper objectMapper = new ObjectMapper();

    @GetMapping("/analyzedPosts")
    public ResponseEntity<List<AnalyzedPostDto>> getPostsByJobId(@RequestParam UUID jobId) {
        return ResponseEntity.ok(analyzedPostService.findAllByJobId(jobId).stream().map(this::map).collect(Collectors.toList()));
    }

    private AnalyzedPostDto map(AnalyzedObjectDto<PostDto, String> post) {
        var internalPostDto = InternalPostDto.builder()
                .authorId(post.getPost().getAuthorId())
                .text(post.getPost().getText())
                .postedAt(Date.from(Instant.parse(post.getPost().getDateTime())))
                .build();

        var analyses = post.getAnalyses().stream().map(this::mapToJsonNode).collect(Collectors.toList());

        return AnalyzedPostDto.builder().jobId(post.getJobId()).postDto(internalPostDto).analyses(analyses).build();
    }

    private JsonNode mapToJsonNode(String content) {
        try {
            return objectMapper.readTree(content);
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }

    @Value
    @Builder
    private static class AnalyzedPostDto {
        private UUID jobId;
        private InternalPostDto postDto;
        private List<JsonNode> analyses;
    }

    @Value
    @Builder
    private static class InternalPostDto {
        private String authorId;
        private String text;
        private Date postedAt; //serialize to string
    }
}
