package cz.cuni.mff.socneto.storage.internal.registrator;

import cz.cuni.mff.socneto.storage.internal.ApplicationProperties;
import cz.cuni.mff.socneto.storage.internal.ComponentProperties;
import cz.cuni.mff.socneto.storage.internal.analyzer.AnalyzerService;
import cz.cuni.mff.socneto.storage.internal.model.RegistrationMessage;
import cz.cuni.mff.socneto.storage.internal.producer.RegistrationProducer;
import lombok.RequiredArgsConstructor;
import org.springframework.boot.context.event.ApplicationReadyEvent;
import org.springframework.context.event.EventListener;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class ComponentRegistrator {

    private final ApplicationProperties applicationProperties;
    private final ComponentProperties componentProperties;
    private final AnalyzerService analyzerService;
    private final RegistrationProducer registrationProducer;

    @EventListener
    public void register(ApplicationReadyEvent event) {
        var registration = new RegistrationMessage();
        registration.setComponentId(componentProperties.getComponentId());
        registration.setComponentType(componentProperties.getComponentType());
        registration.setInputChannelName(componentProperties.getTopicInput());
        registration.setUpdateChannelName(componentProperties.getTopicUpdate());
        registration.setAttributes(new RegistrationMessage.Attributes(analyzerService.getFormat()));

        registrationProducer.send(applicationProperties.getTopicRegistration(), registration);
    }
}
