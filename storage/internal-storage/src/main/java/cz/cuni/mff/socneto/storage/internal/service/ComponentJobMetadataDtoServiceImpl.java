package cz.cuni.mff.socneto.storage.internal.service;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentJobMetadataDto;
import cz.cuni.mff.socneto.storage.internal.api.service.ComponentJobMetadataDtoService;
import cz.cuni.mff.socneto.storage.internal.data.mapper.ComponentJobMetadataMapper;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class ComponentJobMetadataDtoServiceImpl implements ComponentJobMetadataDtoService {

    private final ComponentJobMetadataService componentJobMetadataService;
    private final ComponentJobMetadataMapper componentJobMetadataMapper;

    @Override
    public List<ComponentJobMetadataDto> findAllByComponentId(String componentId) {
        return componentJobMetadataMapper.componentJobMetadatasToJobComponentMetadataDtos(
                ServiceUtils.toList(componentJobMetadataService.findAllByComponentId(componentId)));
    }

    @Override
    public ComponentJobMetadataDto find(String componentId, UUID jobId) {
        return componentJobMetadataMapper
                .componentJobMetadataToJobComponentMetadataDto(
                        componentJobMetadataService.find(componentId, jobId));
    }

    @Override
    public ComponentJobMetadataDto save(ComponentJobMetadataDto componentJobMetadata) {
        return componentJobMetadataMapper
                .componentJobMetadataToJobComponentMetadataDto(
                        componentJobMetadataService.save(componentJobMetadataMapper.componentJobMetadataDtoToJobComponentMetadata(componentJobMetadata)));
    }

    @Override
    public ComponentJobMetadataDto update(ComponentJobMetadataDto componentJobMetadata) {
        return componentJobMetadataMapper
                .componentJobMetadataToJobComponentMetadataDto(
                        componentJobMetadataService.update(componentJobMetadataMapper.componentJobMetadataDtoToJobComponentMetadata(componentJobMetadata)));
    }
}
