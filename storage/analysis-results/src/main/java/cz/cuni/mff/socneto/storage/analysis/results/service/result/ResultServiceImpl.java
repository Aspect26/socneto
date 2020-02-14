package cz.cuni.mff.socneto.storage.analysis.results.service.result;

import cz.cuni.mff.socneto.storage.analysis.results.api.result.request.ResultRequest;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.response.Result;
import cz.cuni.mff.socneto.storage.analysis.results.api.result.validator.ResultRequestValidator;
import cz.cuni.mff.socneto.storage.analysis.results.api.service.ResultService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class ResultServiceImpl implements ResultService {

    private final ResultRequestValidator resultRequestValidator;
    private final ResultRequestDtoProcessorVisitor resultRequestDtoProcessor;

    @Override
    public Result computeResults(ResultRequest request) {
        request.visit(resultRequestValidator);
        return request.visit(resultRequestDtoProcessor);
    }
}
