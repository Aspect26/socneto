package cz.cuni.mff.socneto.storage.internal.service;

import lombok.experimental.UtilityClass;

import java.util.ArrayList;
import java.util.List;

@UtilityClass
class ServiceUtils {

    static <T> List<T> toList(Iterable<T> iterable) {
        var list = new ArrayList<T>();
        iterable.iterator().forEachRemaining(list::add);
        return list;
    }
}
