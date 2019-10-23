package cz.cuni.mff.socneto.storage.analysis.results.api.service;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.AnalyzedObjectDto;
import cz.cuni.mff.socneto.storage.analysis.results.api.dto.PostDto;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface AnalyzedPostDtoService {

    //TODO remove Optional? or add optional to job/user service

    AnalyzedObjectDto<PostDto, String> create(AnalyzedObjectDto<PostDto, String> post);

    Optional<AnalyzedObjectDto<PostDto, String>> findById(UUID id);

    List<AnalyzedObjectDto<PostDto, String>> findAllByJobId(UUID jobId);

    void update(AnalyzedObjectDto<PostDto, String> post);

}
