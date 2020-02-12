package cz.cuni.mff.socneto.storage.logging;

import ch.qos.logback.classic.spi.ILoggingEvent;
import ch.qos.logback.core.encoder.EncoderBase;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import lombok.Builder;
import lombok.Value;

import java.text.SimpleDateFormat;
import java.util.Date;

public class JsonLogEncoder extends EncoderBase<ILoggingEvent> {

    private final SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
    private final ObjectMapper objectMapper = new ObjectMapper();

    private String componentId;

    @Override
    public byte[] headerBytes() {
        return new byte[0];
    }

    @Override
    public byte[] encode(ILoggingEvent event) {
        if (componentId == null) {
            componentId = context.getProperty("COMPONENT_ID");
        }

        var log = Log.builder()
                .componentId(componentId)
                .eventType(event.getLevel().toString())
                .eventName(event.getLevel().toString())
                .message(event.getMessage())
                .timestamp(format.format(new Date(event.getTimeStamp())))
                .build();

        try {
            return objectMapper.writeValueAsString(log).getBytes();
        } catch (JsonProcessingException e) {
            throw new IllegalArgumentException("Serialization error", e);
        }
    }

    @Override
    public byte[] footerBytes() {
        return new byte[0];
    }

    @Value
    @Builder
    private static final class Log {
        private final String componentId;
        private final String eventType;
        private final String eventName;
        private final String message;
        private final String timestamp;
        private final ObjectNode attributes;
    }
}
