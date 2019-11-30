package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchAnalysisDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.SearchPostDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.Result;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.ResultService;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchAnalysisDtoService;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.SearchPostDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequiredArgsConstructor
public class ResultsController {

    private final ResultService resultService;

    @PostMapping("/results")
    public Result computeResults(@RequestBody ResultRequest resultRequest) {
        return resultService.computeResults(resultRequest);
    }

    // LEGACY

    private final SearchPostDtoService searchPostDtoService;
    private final SearchAnalysisDtoService searchAnalysisDtoService;


    @PostMapping("/internal/posts")
    public SearchPostDto create(@RequestBody SearchPostDto searchPostDto) {
        return searchPostDtoService.create(searchPostDto);
    }

    @PostMapping("/internal/analysis")
    public SearchAnalysisDto create(@RequestBody SearchAnalysisDto analysisDto) {
        return searchAnalysisDtoService.create(analysisDto);
    }

}
