package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;
import cz.cuni.mff.socneto.storage.internal.data.model.Component;
import cz.cuni.mff.socneto.storage.internal.repository.ComponentRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import javax.persistence.EntityNotFoundException;

@Service
@RequiredArgsConstructor
public class ComponentService {

    private final ComponentRepository repository;

    public Component find(String id) {
        return repository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("Component with id: " + id + " not found"));
    }

    public Iterable<Component> findAllByType(ComponentType type) {
        return repository.getAllByType(type);
    }

    public Iterable<Component> findAll() {
        return repository.findAll();
    }


    public Component save(Component component) {
        return repository.save(component);
    }

    public Component update(Component component) {
        return repository.save(component);
    }

}
