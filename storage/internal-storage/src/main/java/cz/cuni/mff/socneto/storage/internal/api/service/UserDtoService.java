package cz.cuni.mff.socneto.storage.internal.api.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.UserDto;

public interface UserDtoService {

    UserDto find(String username);

    UserDto save(UserDto user);

}
