package cz.cuni.mff.socneto.storage.internal.repository;

import org.springframework.stereotype.Component;

import java.util.Optional;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.function.Predicate;
import java.util.stream.Collectors;

@Component
public class TempRepository<T> implements InternalRepository<T> {

    private final Set<T> storage = ConcurrentHashMap.newKeySet();

    @Override
    public Optional<T> getByPredicate(Predicate<T> predicate) {
        return storage.stream().filter(predicate).findFirst();
    }

    @Override
    public Set<T> getManyByPredicate(Predicate<T> predicate) {
        return storage.stream().filter(predicate).collect(Collectors.toSet());
    }

    @Override
    public T save(T data) {
        storage.add(data);
        return data;
    }

    @Override
    public boolean delete(T data) {
        return storage.remove(data);
    }
}
