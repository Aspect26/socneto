package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.model.ComponentJobMetadata;
import cz.cuni.mff.socneto.storage.internal.repository.ComponentJobMetadataRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.UUID;

@Service
@RequiredArgsConstructor
public class ComponentJobMetadataService {

    private final ComponentJobMetadataRepository repository;

    public Iterable<ComponentJobMetadata> findAllByComponentId(String componentId) {
        return repository.findAllByComponentId(componentId);
    }

    public ComponentJobMetadata find(String componentId, UUID jobId) {
        return repository.findByComponentIdAndJobId(componentId, jobId);
    }

    public ComponentJobMetadata save(ComponentJobMetadata componentJobMetadata) {
        return repository.save(componentJobMetadata);
    }

    public ComponentJobMetadata update(ComponentJobMetadata componentJobMetadata) {
        var old = find(componentJobMetadata.getComponentId(), componentJobMetadata.getJobId());
        componentJobMetadata.setId(old.getId());
        return repository.save(componentJobMetadata);
    }

}
