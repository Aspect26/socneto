package cz.cuni.mff.socneto.storage.analysis.storage;

import cz.cuni.mff.socneto.storage.analysis.storage.repository.AnalyzedPostRepository;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.mongodb.repository.config.EnableMongoRepositories;

@Configuration
@EnableMongoRepositories(basePackageClasses = AnalyzedPostRepository.class)
public class AnalysisStorageConfiguration {
}
