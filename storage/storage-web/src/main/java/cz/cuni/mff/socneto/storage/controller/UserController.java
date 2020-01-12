package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.UserDto;
import cz.cuni.mff.socneto.storage.internal.api.service.UserDtoService;
import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;
import lombok.extern.slf4j.Slf4j;
import org.springframework.web.bind.annotation.*;

@Slf4j
@RestController
@RequiredArgsConstructor
public class UserController {

    private final UserDtoService userDtoService;

    @GetMapping("/users")
    public UserDto getUserByUsername(@RequestParam("username") String username) {
        log.error("Errror: " + username);
        return userDtoService.find(username);
    }

    @PostMapping("/users")
    public UserDto saveUser(@RequestBody UserDto user) {
        return userDtoService.save(user);
    }

}
