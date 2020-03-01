package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobConfigDto;
import org.junit.Assert;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.junit4.SpringRunner;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.Random;
import java.util.UUID;

@RunWith(SpringRunner.class)
@SpringBootTest
public class ComponentJobControllerTest {

    @Autowired
    private ComponentJobConfigController componentJobController;

    @Test
    public void testCreateComponentConfig() {
        var component = ComponentJobConfigDto.builder()
                .componentId("component_" + new Random().nextLong())
                .jobId(UUID.randomUUID())
                .outputChannelNames(List.of("test"))
                .build();

        ComponentJobConfigDto createdComponent = componentJobController.createJobComponentMetadata(component.getComponentId(), component);

        component.setId(createdComponent.getId());
        Assert.assertEquals(component, createdComponent);
    }

    @Test
    @Transactional
    public void testGetComponentConfigByJob() {
        var component = ComponentJobConfigDto.builder()
                .componentId("component_" + new Random().nextLong())
                .jobId(UUID.randomUUID())
                .outputChannelNames(List.of("test"))
                .build();

        ComponentJobConfigDto createdComponent = componentJobController.createJobComponentMetadata(component.getComponentId(), component);

        List<ComponentJobConfigDto> jobComponentMetadata = componentJobController.getJobComponentMetadata(component.getComponentId());

        Assert.assertTrue(jobComponentMetadata.size() == 1);
        Assert.assertEquals(createdComponent, jobComponentMetadata.get(0));

        ComponentJobConfigDto jobComponentMetadataByJob = componentJobController.getJobComponentMetadataByJob(component.getComponentId(), component.getJobId());

        Assert.assertEquals(createdComponent, jobComponentMetadataByJob);
    }
}