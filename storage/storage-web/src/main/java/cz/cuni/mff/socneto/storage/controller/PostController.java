package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.ListWithCount;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchPostDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchPostDtoService;
import lombok.Data;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import java.util.Date;
import java.util.List;
import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class PostController {

    private final SearchPostDtoService searchPostDtoService;

    @PostMapping("/analyzedPosts")
    public ResponseEntity<ListWithCount<SearchPostDto>> getPosts(@RequestBody PostSearchRequest postSearchRequest) {
        return ResponseEntity.ok(searchPostDtoService.searchPosts(postSearchRequest.getJobId(),
                postSearchRequest.getAllowedTerms(),
                postSearchRequest.getForbiddenTerms(),
                postSearchRequest.getPage(),
                postSearchRequest.getSize(),
                postSearchRequest.getFromDate(),
                postSearchRequest.getToDate()));
    }

    @Data
    static class PostSearchRequest {
        private UUID jobId;
        private List<String> allowedTerms;
        private List<String> forbiddenTerms;
        private int page = 0;
        private int size = 100;
        private Date fromDate;
        private Date toDate;
    }
}
