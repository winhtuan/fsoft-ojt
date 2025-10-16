using System.Linq;
using AutoMapper;
using Plantpedia.DTO;
using Plantpedia.Enum;
using Plantpedia.Models;

public class UserMapProfile : Profile
{
    public UserMapProfile()
    {
        CreateMap<UserAccount, AdminUserListItemDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.LoginData.Username))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.LoginData.Email))
            .ForMember(d => d.LastLoginAt, o => o.MapFrom(s => s.LoginData.LastLoginAt))
            .ForMember(d => d.CommentCount, o => o.MapFrom(s => s.PlantComments.Count))
            .ForMember(d => d.ReactionCount, o => o.MapFrom(s => s.PlantCommentReactions.Count))
            .ForMember(
                d => d.SearchCount,
                o => o.MapFrom(s => s.Activities.Count(a => a.Type == ActivityType.Search))
            );
    }
}
