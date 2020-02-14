package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobMetadataDto;
import cz.cuni.mff.socneto.storage.internal.api.service.ComponentJobMetadataDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import javax.validation.Valid;
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
                                                              @RequestBody @Valid ComponentJobMetadataDto metadataDto) {
        metadataDto.setComponentId(componentId);
        return componentJobMetadataDtoService.save(metadataDto);
    }

    @PutMapping("/components/{componentId}/metadata")
    public ComponentJobMetadataDto updateJobComponentMetadata(@PathVariable String componentId,
                                                              @RequestBody @Valid ComponentJobMetadataDto metadataDto) {
        metadataDto.setComponentId(componentId);
        return componentJobMetadataDtoService.update(metadataDto);
    }

}
