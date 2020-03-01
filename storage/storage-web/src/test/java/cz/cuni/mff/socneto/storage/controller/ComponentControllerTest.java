package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentDto;
import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;
import org.junit.Assert;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.junit4.SpringRunner;

import java.util.UUID;

@RunWith(SpringRunner.class)
@SpringBootTest
public class ComponentControllerTest {

    @Autowired
    private ComponentController componentController;

    @Test
    public void testSingleComponent() {
        var componentDto = ComponentDto.builder()
                .componentId(UUID.randomUUID().toString())
                .type(ComponentType.DATA_ANALYSER)
                .inputChannelName("input")
                .updateChannelName("update")
                .build();

        var createdComponent = componentController.createComponent(componentDto);

        var getComponent = componentController.getComponent(componentDto.getComponentId());

        Assert.assertEquals(componentDto, createdComponent);
        Assert.assertEquals(componentDto, getComponent);
    }

    @Test
    public void testListComponents() {
        var componentDto1 = ComponentDto.builder()
                .componentId(UUID.randomUUID().toString())
                .type(ComponentType.DATA_ANALYSER)
                .inputChannelName("input")
                .updateChannelName("update")
                .build();

        var componentDto2 = ComponentDto.builder()
                .componentId(UUID.randomUUID().toString())
                .type(ComponentType.DATA_ACQUIRER)
                .inputChannelName("input")
                .updateChannelName("update")
                .build();

        var createdComponent1 = componentController.createComponent(componentDto1);
        var createdComponent2 = componentController.createComponent(componentDto2);

        Assert.assertEquals(componentDto1, createdComponent1);
        Assert.assertEquals(componentDto2, createdComponent2);

        var all = componentController.getComponentsByType(null);

        Assert.assertTrue(all.contains(componentDto1));
        Assert.assertTrue(all.contains(componentDto2));

        var analysers = componentController.getComponentsByType(ComponentType.DATA_ANALYSER);

        Assert.assertTrue(analysers.contains(componentDto1));
        Assert.assertFalse(analysers.contains(componentDto2));
    }

}