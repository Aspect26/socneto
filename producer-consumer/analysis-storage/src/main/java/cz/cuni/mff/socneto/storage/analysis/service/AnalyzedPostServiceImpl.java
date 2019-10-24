package cz.cuni.mff.socneto.storage.analysis.service;

import cz.cuni.mff.socneto.storage.analysis.data.dto.AnalyzedObjectDto;
import cz.cuni.mff.socneto.storage.analysis.data.dto.PostDto;
import cz.cuni.mff.socneto.storage.analysis.data.mapper.PostMapper;
import cz.cuni.mff.socneto.storage.analysis.data.model.AnalyzedObject;
import cz.cuni.mff.socneto.storage.analysis.data.model.Post;
import cz.cuni.mff.socneto.storage.analysis.repository.AnalyzedPostRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.UUID;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class AnalyzedPostServiceImpl implements AnalyzedPostService {

    private final AnalyzedPostRepository analyzedPostRepository;
    private final PostMapper postMapper;

    @Override
    public AnalyzedObjectDto<PostDto, String> create(AnalyzedObjectDto<PostDto, String> post) {
        return mapToDto(analyzedPostRepository.insert(mapFromDto(post)));
    }

    @Override
    public Optional<AnalyzedObjectDto<PostDto, String>> findById(UUID id) {
        return analyzedPostRepository.findById(id).map(this::mapToDto);
    }

    @Override
    public List<AnalyzedObjectDto<PostDto, String>> findAllByJobId(UUID jobId) {
        return analyzedPostRepository.findAllByJobId(jobId).stream().map(this::mapToDto).collect(Collectors.toList());
    }

    @Override
    public void update(AnalyzedObjectDto<PostDto, String> post) {
        analyzedPostRepository.save(mapFromDto(post));
    }

    private AnalyzedObjectDto<PostDto, String> mapToDto(AnalyzedObject<Post, String> post) {
        return AnalyzedObjectDto.<PostDto, String>builder().id(post.getId())
                .jobId(post.getJobId()).analyses(post.getAnalyses()).post(postMapper.postToPostDto(post.getPost())).build();
    }

    private AnalyzedObject<Post, String> mapFromDto(AnalyzedObjectDto<PostDto, String> post) {
        return AnalyzedObject.<Post, String>builder().id(post.getId())
                .jobId(post.getJobId()).analyses(post.getAnalyses()).post(postMapper.postDtoToPost(post.getPost())).build();
    }
}
