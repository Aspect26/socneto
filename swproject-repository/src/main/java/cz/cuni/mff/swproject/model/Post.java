package cz.cuni.mff.swproject.model;

import lombok.*;
import org.springframework.data.annotation.Id;

import javax.persistence.Column;
import javax.persistence.PrePersist;
import java.util.List;
import java.util.UUID;

@Builder
@Getter
@Setter
public class Post {

    @Id
    private UUID id;

    // TODO explore nullable
    @Column(nullable = false)
    private String originalId;

    @Column(nullable = false)
    private String collection;

    @Column(nullable = false)
    private String text;

    @Column(nullable = false)
    private List<String> keywords;


    // TODO explore this option
    // @PrePersist
    // private void generateSecret(){
    //    this.setId(UUID.randomUUID());
    // }
}
