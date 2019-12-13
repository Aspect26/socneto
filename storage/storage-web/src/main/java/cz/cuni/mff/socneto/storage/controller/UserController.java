package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.UserDto;
import cz.cuni.mff.socneto.storage.internal.api.service.UserDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

@RestController
@RequiredArgsConstructor
public class UserController {

    private final UserDtoService userDtoService;

    @GetMapping("/users")
    public UserDto getUserByUsername(@RequestParam("username") String username) {
        return userDtoService.find(username);
    }

    @PostMapping("/users")
    public UserDto saveUser(@RequestBody UserDto user) {
        return userDtoService.save(user);
    }

}
