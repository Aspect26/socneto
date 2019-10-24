package cz.cuni.mff.socneto.storage.analysis.data.mapper;

import cz.cuni.mff.socneto.storage.analysis.data.dto.PostDto;
import cz.cuni.mff.socneto.storage.analysis.data.model.Post;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.factory.Mappers;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR)
public interface PostMapper {
    PostMapper INSTANCE = Mappers.getMapper(PostMapper.class);

    Post postDtoToPost(PostDto postDto);

    PostDto postToPostDto(Post post);
}