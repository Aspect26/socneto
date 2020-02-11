package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.UserDto;
import cz.cuni.mff.socneto.storage.internal.api.service.UserDtoService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import javax.validation.Valid;

@Slf4j
@RestController
@RequiredArgsConstructor
public class UserController {

    private final UserDtoService userDtoService;

    @GetMapping("/users")
    public UserDto getUserByUsername(@RequestParam("username") String username) {
        return userDtoService.find(username);
    }

    @PostMapping("/users")
    public UserDto saveUser(@Valid @RequestBody UserDto user) {
        return userDtoService.save(user);
    }

}
