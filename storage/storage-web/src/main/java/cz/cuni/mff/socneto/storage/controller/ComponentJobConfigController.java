package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobConfigDto;
import cz.cuni.mff.socneto.storage.internal.api.service.ComponentJobConfigDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import javax.validation.Valid;
import java.util.List;
import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class ComponentJobConfigController {

    private final ComponentJobConfigDtoService componentJobMetadataDtoService;

    @GetMapping("/components/{componentId}/configs")
    public List<ComponentJobConfigDto> getJobComponentMetadata(@PathVariable String componentId) {
        return componentJobMetadataDtoService.findAllByComponentId(componentId);
    }

    @GetMapping("/components/{componentId}/configs/job/{jobId}")
    public ComponentJobConfigDto getJobComponentMetadataByJob(@PathVariable String componentId, @PathVariable UUID jobId) {
        return componentJobMetadataDtoService.find(componentId, jobId);
    }

    @PostMapping("/components/{componentId}/configs")
    public ComponentJobConfigDto createJobComponentMetadata(
            @PathVariable String componentId, @RequestBody @Valid ComponentJobConfigDto componentJobConfigDto
    ) {
        componentJobConfigDto.setComponentId(componentId);
        return componentJobMetadataDtoService.save(componentJobConfigDto);
    }

}
