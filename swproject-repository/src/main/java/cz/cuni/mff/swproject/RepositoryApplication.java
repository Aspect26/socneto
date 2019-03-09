package cz.cuni.mff.swproject;

import cz.cuni.mff.swproject.repositories.PostRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
public class RepositoryApplication {

    @Autowired
    PostRepository repository;

    public static void main(String[] args) {
        SpringApplication.run(RepositoryApplication.class, args);
    }
}
