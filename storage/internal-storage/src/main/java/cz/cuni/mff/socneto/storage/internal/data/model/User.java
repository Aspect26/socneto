package cz.cuni.mff.socneto.storage.internal.data.model;

import lombok.Data;

import javax.persistence.Entity;
import javax.persistence.Id;

@Data
@Entity(name = "internal_user")
public class User {
    @Id
    private String username;
    private String password;
}
