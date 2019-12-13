package cz.cuni.mff.socneto.storage.controller;

import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentDto;
import cz.cuni.mff.socneto.storage.internal.api.dto.ComponentType;
import cz.cuni.mff.socneto.storage.internal.api.service.ComponentDtoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequiredArgsConstructor
public class ComponentController {

    private final ComponentDtoService componentDtoService;

    @GetMapping("/components/{id}")
    public ComponentDto getComponent(@PathVariable("id") String id) {
        return componentDtoService.find(id);
    }

    @GetMapping("/components")
    public List<ComponentDto> getComponentsByType(@RequestParam(value = "type", required = false) ComponentType type) {
        if (type == null) {
            return componentDtoService.findAll();
        } else {
            return componentDtoService.findAllByType(type);
        }
    }

    @PostMapping("/components")
    public ComponentDto createComponent(@RequestBody ComponentDto componentDto) {
        return componentDtoService.save(componentDto);
    }

    @PutMapping("/components/{id}")
    public ComponentDto updateComponent(@RequestBody ComponentDto componentDto) {
        return componentDtoService.update(componentDto);
    }
}
