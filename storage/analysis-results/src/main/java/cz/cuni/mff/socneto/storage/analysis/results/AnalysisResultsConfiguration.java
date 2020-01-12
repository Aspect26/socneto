package cz.cuni.mff.socneto.storage.analysis.results;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ArrayNode;
import lombok.extern.slf4j.Slf4j;
import org.apache.http.HttpHost;
import org.elasticsearch.action.admin.cluster.health.ClusterHealthRequest;
import org.elasticsearch.action.admin.cluster.storedscripts.PutStoredScriptRequest;
import org.elasticsearch.client.Client;
import org.elasticsearch.client.RequestOptions;
import org.elasticsearch.client.RestClient;
import org.elasticsearch.client.RestHighLevelClient;
import org.elasticsearch.client.transport.TransportClient;
import org.elasticsearch.common.bytes.BytesArray;
import org.elasticsearch.common.settings.Settings;
import org.elasticsearch.common.transport.TransportAddress;
import org.elasticsearch.common.xcontent.XContentType;
import org.elasticsearch.transport.client.PreBuiltTransportClient;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.elasticsearch.core.ElasticsearchOperations;
import org.springframework.data.elasticsearch.core.ElasticsearchTemplate;
import org.springframework.data.elasticsearch.repository.config.EnableElasticsearchRepositories;

import javax.annotation.PostConstruct;
import java.io.IOException;
import java.net.InetAddress;
import java.net.UnknownHostException;

@Slf4j
@Configuration
@EnableElasticsearchRepositories(basePackages = "cz.cuni.mff.socneto.storage.analysis.results.repository")
public class AnalysisResultsConfiguration {

    @Value("${elasticsearch.host}")
    private String host;

    @Value("${elasticsearch.port}")
    private int port;

    @Value("${elasticsearch.wait.millis}")
    private int wait;

    @Bean
    public TransportClient client() throws UnknownHostException {
        return new PreBuiltTransportClient(Settings.builder().put("cluster.name", "docker-cluster").build())
                .addTransportAddress(new TransportAddress(InetAddress.getByName(host), port));
    }

    @Bean
    public ElasticsearchOperations elasticsearchTemplate(Client client) {
        return new ElasticsearchTemplate(client);
    }

    @Bean
    public RestHighLevelClient heightClient() {
        return new RestHighLevelClient(
                RestClient.builder(
                        new HttpHost(host, 9200, "http")
                )
        );
    }


    // Move away

    @PostConstruct
    void init() throws IOException, InterruptedException {
        Thread.sleep(wait);

        log.info("Loading scripts");

        RestHighLevelClient restHighLevelClient = heightClient();
        ObjectMapper objectMapper = new ObjectMapper();


        var resource = this.getClass().getResource("scripts.json");
        var array = (ArrayNode) objectMapper.readTree(resource);

        array.forEach(node -> {
            var putStoredScriptRequest =
                    new PutStoredScriptRequest().id(node.get("name").asText()).content(new BytesArray(node.get("value").toString()),
                            XContentType.JSON);
            try {
                var acknowledgedResponse = restHighLevelClient.putScript(putStoredScriptRequest, RequestOptions.DEFAULT);
                if (!acknowledgedResponse.isAcknowledged()) {
                    throw new IllegalStateException("Not accepted script: " + node.get("name").asText());
                }
            } catch (IOException e) {
                throw new IllegalStateException("Script: " + node.get("name").asText() + ": " + e.getMessage());
            }
        });

        log.info("Scripts loaded.");
    }
}
