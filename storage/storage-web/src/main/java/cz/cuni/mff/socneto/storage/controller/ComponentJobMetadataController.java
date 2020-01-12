package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobMetadataDto;
import cz.cuni.mff.socneto.storage.internal.api.service.ComponentJobMetadataDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class ComponentJobMetadataController {

    private final ComponentJobMetadataDtoService componentJobMetadataDtoService;

    @GetMapping("/components/{componentId}/metadata/job/{jobId}")
    public ComponentJobMetadataDto getJobComponentMetadata(@PathVariable String componentId, @PathVariable UUID jobId) {
        return componentJobMetadataDtoService.find(componentId, jobId);
    }

    @PostMapping("/components/{componentId}/metadata")
    public ComponentJobMetadataDto createJobComponentMetadata(@PathVariable String componentId,
                                                              @RequestBody ComponentJobMetadataDto metadataDto) {
        metadataDto.setComponentId(componentId);
        return componentJobMetadataDtoService.save(metadataDto);
    }

    @PutMapping("/components/{componentId}/metadata")
    public ComponentJobMetadataDto updateJobComponentMetadata(@PathVariable String componentId,
                                                              @RequestBody ComponentJobMetadataDto metadataDto) {
        metadataDto.setComponentId(componentId);
        return componentJobMetadataDtoService.update(metadataDto);
    }

}
