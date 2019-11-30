package cz.cuni.mff.socneto.storage.analysis.storage.api.validation;

import lombok.experimental.UtilityClass;

import java.util.Objects;

@UtilityClass
public class ValidationUtils {

    public static void validateNotNull(Object object, String name) {
        if (Objects.isNull(object)) {
            throw new IllegalArgumentException("Object " + name + " can't be null.");
        }
    }
}
