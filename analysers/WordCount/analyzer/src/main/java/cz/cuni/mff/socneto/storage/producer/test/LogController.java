package cz.cuni.mff.socneto.storage.producer.test;

import cz.cuni.mff.socneto.storage.model.LogMessage;
import cz.cuni.mff.socneto.storage.producer.LogProducer;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequiredArgsConstructor
public class LogController {

    private final LogProducer sender;

    @PostMapping("internal/log")
    public void log(@RequestParam String text) {
        sender.send(new LogMessage(text));
    }

}
