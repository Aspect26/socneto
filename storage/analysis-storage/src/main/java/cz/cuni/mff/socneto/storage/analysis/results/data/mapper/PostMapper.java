package cz.cuni.mff.socneto.storage.analysis.results.data.mapper;

import cz.cuni.mff.socneto.storage.analysis.results.api.dto.PostDto;
import cz.cuni.mff.socneto.storage.analysis.results.data.model.Post;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface PostMapper {
//    PostMapper INSTANCE = Mappers.getMapper(PostMapper.class);

    Post postDtoToPost(PostDto postDto);

    PostDto postToPostDto(Post post);
}