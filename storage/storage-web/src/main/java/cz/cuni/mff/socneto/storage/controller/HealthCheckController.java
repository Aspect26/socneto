package cz.cuni.mff.socneto.storage.controller;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class HealthCheckController {

    @GetMapping("/health-check")
    public String check() {
        return "Healthy as never before!";
    }
}
