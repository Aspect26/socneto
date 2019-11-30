package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.JobComponentMetadataDto;
import cz.cuni.mff.socneto.storage.internal.api.service.JobComponentMetadataDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@RestController
@RequiredArgsConstructor
public class JobComponentMetadataController {

    private final JobComponentMetadataDtoService jobComponentMetadataDtoService;

    @GetMapping("/components/{componentId}/metadata")
    public List<JobComponentMetadataDto> getJobComponentMetadata(@PathVariable String componentId) {
        return jobComponentMetadataDtoService.findUnfinished(componentId);
    }

    @GetMapping("/components/{componentId}/metadata/job/{jobId}")
    public JobComponentMetadataDto getJobComponentMetadata(@PathVariable String componentId, @PathVariable UUID jobId) {
        return jobComponentMetadataDtoService.find(componentId, jobId);
    }

    @PostMapping("/components/{componentId}/metadata")
    public JobComponentMetadataDto createJobComponentMetadata(@PathVariable String componentId,
                                              @RequestBody JobComponentMetadataDto metadataDto) {
        metadataDto.setComponentId(componentId);
        return jobComponentMetadataDtoService.save(metadataDto);
    }

    @PutMapping("/components/{componentId}/metadata")
    public JobComponentMetadataDto updateJobComponentMetadata(@PathVariable String componentId,
                                              @RequestBody JobComponentMetadataDto metadataDto) {
        metadataDto.setComponentId(componentId);
        return jobComponentMetadataDtoService.update(metadataDto);
    }

}
