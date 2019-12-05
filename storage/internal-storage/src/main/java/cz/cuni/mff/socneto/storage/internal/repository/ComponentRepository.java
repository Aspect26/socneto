package cz.cuni.mff.socneto.storage.internal.repository;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;
import cz.cuni.mff.socneto.storage.internal.data.model.Component;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface ComponentRepository extends CrudRepository<Component, String> {

    Iterable<Component> getAllByType(ComponentType type);

}
