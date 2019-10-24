package cz.cuni.mff.socneto.storage.internal;

import cz.cuni.mff.socneto.storage.internal.data.model.Job;
import cz.cuni.mff.socneto.storage.internal.repository.JobRepository;
import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;

@Configuration
@EnableJpaRepositories(basePackageClasses = JobRepository.class)
@EntityScan(basePackageClasses = Job.class)
public class InternalStorageConfiguration {
}
