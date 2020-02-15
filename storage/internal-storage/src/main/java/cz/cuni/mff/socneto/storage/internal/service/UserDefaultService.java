package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.data.model.User;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.event.ContextRefreshedEvent;
import org.springframework.context.event.EventListener;
import org.springframework.stereotype.Component;

import java.util.Arrays;

@Component
@RequiredArgsConstructor
public class UserDefaultService {

    private final UserService userService;

    @Value("${default.users}")
    private String users;

    @EventListener(ContextRefreshedEvent.class)
    public void createDefaultUsers() {
        Arrays.stream(users.split(";"))
                .map(u -> new User(u.split(":")[0], u.split(":")[1]))
                .forEach(userService::save);
        users = null;
    }
}
