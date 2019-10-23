package cz.cuni.mff.socneto.storage.internal.data.mapper;

import cz.cuni.mff.socneto.storage.internal.api.dto.UserDto;
import cz.cuni.mff.socneto.storage.internal.data.model.User;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

import java.util.List;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface UserMapper {

    User userDtoToUser(UserDto userDto);

    List<User> userDtosToUsers(List<UserDto> userDtos);

    UserDto userToUserDto(User user);

    List<UserDto> usersToUserDtos(List<User> userDtos);
}
