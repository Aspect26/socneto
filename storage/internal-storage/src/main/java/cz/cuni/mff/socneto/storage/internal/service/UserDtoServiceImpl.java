package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.UserDto;
import cz.cuni.mff.socneto.storage.internal.api.service.UserDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.UserMapper;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class UserDtoServiceImpl implements UserDtoService {

    private final UserService userService;
    private final UserMapper userMapper;

    @Override
    public UserDto find(String username) {
        // TODO validate
        return userMapper.userToUserDto(userService.find(username));
    }

    @Override
    public UserDto save(UserDto user) {
        // TODO validate
        return userMapper.userToUserDto(userService.save(userMapper.userDtoToUser(user)));
    }

}
