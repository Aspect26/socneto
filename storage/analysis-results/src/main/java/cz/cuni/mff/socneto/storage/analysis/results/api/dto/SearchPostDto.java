package cz.cuni.mff.socneto.storage.analysis.results.api.dto;

import lombok.Data;

import java.util.Date;
import java.util.UUID;

@Data
public class SearchPostDto {
    private UUID id;
    private String originalId;
    private String text;
    private Date datetime;
}
