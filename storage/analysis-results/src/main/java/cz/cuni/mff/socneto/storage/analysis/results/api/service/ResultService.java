package cz.cuni.mff.socneto.storage.analysis.results.api.service;

import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.Result;

public interface ResultService {

    Result computeResults(ResultRequest request);
}
