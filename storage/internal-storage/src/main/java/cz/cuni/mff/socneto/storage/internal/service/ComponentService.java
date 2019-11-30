package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;
import cz.cuni.mff.socneto.storage.internal.data.model.Component;
import cz.cuni.mff.socneto.storage.internal.repository.ComponentRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import javax.persistence.EntityNotFoundException;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class ComponentService {

    private final ComponentRepository repository;

    public Component find(String id) {
        return repository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("Component with id: " + id + " not found"));
    }

    public Component find(String componentId, UUID jobId) {
        // TODO validate
        return repository.findByComponentIdAndJobId(componentId, jobId)
                .orElseThrow(() -> new EntityNotFoundException("Component with id: " + componentId + "and job id: " + jobId + " not found"));
    }


    public Iterable<Component> getAllByType(ComponentType type) {
        return repository.getAllByType(type);
    }

    public Component save(Component component) {
        return repository.save(component);
    }

    public Component update(Component component) {
        return repository.save(component);
    }

    public void delete(String id) {
        repository.deleteById(id);
    }

}
