package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.dto.UserDto;
import cz.cuni.mff.socneto.storage.internal.repository.InternalRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.web.bind.annotation.RequestBody;

import javax.annotation.PostConstruct;

@Service
@RequiredArgsConstructor
public class UserService {

    private final InternalRepository<UserDto> repository;

    @PostConstruct
    public void init() {
        repository.save(new UserDto("admin", "admin"));
    }

    public UserDto getUserById(String userId) {
        return repository.getByPredicate(user -> user.getUsername().equals(userId)).get();
    }

    public UserDto saveUser(@RequestBody UserDto user) {
        repository.getByPredicate(u -> u.getUsername().equals(user.getUsername())).ifPresent(repository::delete);
        return repository.save(user);
    }

}
