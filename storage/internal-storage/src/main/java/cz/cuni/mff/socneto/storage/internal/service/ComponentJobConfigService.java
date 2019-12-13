package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.model.ComponentJobConfig;
import cz.cuni.mff.socneto.storage.internal.repository.ComponentConfigRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import javax.persistence.EntityNotFoundException;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class ComponentJobConfigService {

    private final ComponentConfigRepository componentConfigRepository;
    
    public ComponentJobConfig find(Long id) {
        return componentConfigRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("Component config with id: " + id + " not found"));
    }

    public ComponentJobConfig find(String componentId, UUID jobId) {
        return componentConfigRepository.findByComponentIdAndJobId(componentId, jobId)
                .orElseThrow(() -> new EntityNotFoundException("component config with component id: " + componentId
                        + "and job id: " + jobId + " not found"));
    }

    public Iterable<ComponentJobConfig> findAllByComponentId(String componentId) {
        return componentConfigRepository.findAllByComponentId(componentId);
    }

    public ComponentJobConfig save(ComponentJobConfig componentJobConfig) {
        return componentConfigRepository.save(componentJobConfig);
    }

    public ComponentJobConfig update(ComponentJobConfig componentJobConfig) {
        return componentConfigRepository.save(componentJobConfig);
    }

    public void delete(String id) {
        throw new UnsupportedOperationException("Not implemented.");
    }
}
