package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.UserDto;
import cz.cuni.mff.socneto.storage.internal.api.service.UserDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import javax.websocket.server.PathParam;

@RestController
@RequiredArgsConstructor
public class UserController {

    private final UserDtoService userDtoService;

    @GetMapping("/users")
    public UserDto getUserById(@RequestParam("userId") String userId) {
        return userDtoService.find(userId);
    }

    @PostMapping("/users")
    public UserDto saveUser(@RequestBody UserDto user) {
        return userDtoService.save(user);
    }

}
