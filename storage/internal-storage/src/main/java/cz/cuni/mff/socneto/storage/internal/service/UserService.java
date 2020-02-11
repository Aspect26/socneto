package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.model.User;
import cz.cuni.mff.socneto.storage.internal.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import javax.persistence.EntityNotFoundException;

@Service
@RequiredArgsConstructor
public class UserService {

    private final UserRepository repository;

    public User find(String username) {
        return repository.findByUsername(username)
                .orElseThrow(() -> new EntityNotFoundException("User with id: " + username + " not found"));
    }

    public User save(User user) {
        return repository.save(user);
    }

    public User update(User user) {
        return repository.save(user);
    }

    public void delete(String username) {
        repository.deleteById(username);
    }
}
