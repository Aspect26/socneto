package cz.cuni.mff.socneto.storage.analysis.storage.repository;

import cz.cuni.mff.socneto.storage.analysis.storage.data.model.AnalyzedObject;
import cz.cuni.mff.socneto.storage.analysis.storage.data.model.Post;
import org.springframework.data.mongodb.core.mapping.event.AbstractMongoEventListener;
import org.springframework.data.mongodb.core.mapping.event.BeforeConvertEvent;
import org.springframework.stereotype.Component;

import java.util.UUID;

@Component
public class AnalyzedObjectEventListener extends AbstractMongoEventListener<AnalyzedObject<Post, String>> {

    @Override
    public void onBeforeConvert(BeforeConvertEvent<AnalyzedObject<Post, String>> event) {
        var customer = event.getSource();
        if (customer.getId() == null) {
            customer.setId(UUID.randomUUID());
        }
    }
}
