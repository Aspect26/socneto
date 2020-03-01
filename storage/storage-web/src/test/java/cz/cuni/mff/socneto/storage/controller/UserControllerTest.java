package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.UserDto;
import org.junit.Assert;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.junit4.SpringRunner;

@RunWith(SpringRunner.class)
@SpringBootTest
public class UserControllerTest {

    @Autowired
    private UserController userController;

    @Test
    public void testGetUserByUsername() {

        UserDto admin = userController.getUserByUsername("admin");

        Assert.assertEquals(admin.getUsername(), "admin");
        Assert.assertTrue(admin.getPassword() != null);
    }

    @Test
    public void testSaveUser() {
        UserDto newUser = UserDto.builder().username("test").password("test").build();

        UserDto createdUser = userController.saveUser(newUser);

        Assert.assertEquals(newUser, createdUser);

        Assert.assertEquals(newUser, userController.getUserByUsername("test"));
    }
}