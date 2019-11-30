package cz.cuni.mff.socneto.storage.analysis.results.api.result.response;

import lombok.Getter;

@Getter
abstract class AbstractResult implements Result {

    private final String resultName;

    AbstractResult(String resultName) {
        this.resultName = resultName;
    }
}
