package cz.cuni.mff.socneto.storage.analysis.results.repository;

import cz.cuni.mff.socneto.storage.analysis.results.data.model.AnalyzedObject;
import cz.cuni.mff.socneto.storage.analysis.results.data.model.Post;
import org.springframework.data.mongodb.repository.MongoRepository;

import java.util.List;
import java.util.UUID;

public interface AnalyzedPostRepository extends MongoRepository<AnalyzedObject<Post, String>, UUID> {

    List<AnalyzedObject<Post, String>> findAllByJobId(UUID jobId);
}
