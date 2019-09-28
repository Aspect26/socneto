package cz.cuni.mff.socneto.storage.internal.data.dto;

import lombok.Value;

@Value
public class UserDto {
    private String username;
    private String password;
}
