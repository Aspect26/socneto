package cz.cuni.mff.socneto.storage.analysis.service;

import cz.cuni.mff.socneto.storage.analysis.data.dto.AnalyzedObjectDto;
import cz.cuni.mff.socneto.storage.analysis.data.dto.PostDto;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface AnalyzedPostService {

    AnalyzedObjectDto<PostDto, String> create(AnalyzedObjectDto<PostDto, String> post);

    Optional<AnalyzedObjectDto<PostDto, String>> findById(UUID id);

    List<AnalyzedObjectDto<PostDto, String>> findAllByJobId(UUID jobId);

    void update(AnalyzedObjectDto<PostDto, String> post);

}
