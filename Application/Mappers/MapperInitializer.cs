using Application.DTOs;
using AutoMapper;
using Core.Entities;
using System.Linq;

namespace Application.Mappers
{
    public class MapperInitializer
    {
        public static void Initialize()
        {

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<StepBlock, StepBlockDTO>()
                  .ForMember(
                       dest => dest.Questions,
                          opt => opt.MapFrom(src =>
                              src.Questions.OrderBy(q => q.Order).Select(
                                  q => Mapper.Map<QuestionDTO>(q))));

                cfg.CreateMap<Question, QuestionDTO>()
                  .ForMember(
                       dest => dest.Options,
                          opt => opt.MapFrom(src =>
                              src.HasOptions ? src.Options.Select(
                                  o => Mapper.Map<OptionDTO>(o)) : null));

                cfg.CreateMap<User, UserDTO>()
                 .ForMember(
                      dest => dest.Position,
                         opt => opt.MapFrom(src =>
                             src.Position.Title));

            });
        }
    }
}
