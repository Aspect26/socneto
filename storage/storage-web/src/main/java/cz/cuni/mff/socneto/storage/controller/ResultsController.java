package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.Result;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.ResultService;
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

}
