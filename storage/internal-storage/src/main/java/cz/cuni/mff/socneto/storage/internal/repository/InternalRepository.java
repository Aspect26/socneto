package cz.cuni.mff.socneto.storage.internal.repository;

import java.util.Optional;
import java.util.Set;
import java.util.function.Predicate;

public interface InternalRepository<T> {

    Optional<T> getByPredicate(Predicate<T> predicate);

    Set<T> getManyByPredicate(Predicate<T> predicate);

    T save(T data);

    boolean delete(T data);
}
