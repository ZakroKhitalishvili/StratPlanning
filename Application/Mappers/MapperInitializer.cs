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

                cfg.CreateMap<Question, QuestionDTO>()
                  .ForMember(
                       dest => dest.Files,
                          opt => opt.MapFrom(src =>
                              src.HasFiles ? src.Files.Select(
                                  o => Mapper.Map<FileDTO>(o)) : null));

                cfg.CreateMap<User, UserDTO>()
                 .ForMember(
                      dest => dest.Position,
                         opt => opt.MapFrom(src =>
                             src.Position));

                cfg.CreateMap<NewUserDTO, User>()
                .ForMember(
                     dest => dest.Id,
                        opt => opt.Ignore())
                .ForMember(
                     dest => dest.CreatedAt,
                        opt => opt.Ignore())
                .ForMember(
                     dest => dest.CreatedBy,
                        opt => opt.Ignore())
                .ForMember(
                     dest => dest.UpdatedAt,
                        opt => opt.Ignore())
               .ForMember(
                     dest => dest.UpdatedBy,
                        opt => opt.Ignore());

                cfg.CreateMap<DictionaryDTO, Dictionary>()
                .ForMember(
                     dest => dest.Id,
                        opt => opt.Ignore())
                .ForMember(
                     dest => dest.CreatedAt,
                        opt => opt.Ignore())
                .ForMember(
                     dest => dest.CreatedBy,
                        opt => opt.Ignore())
                .ForMember(
                     dest => dest.UpdatedAt,
                        opt => opt.Ignore())
               .ForMember(
                     dest => dest.UpdatedBy,
                        opt => opt.Ignore());

            });
        }
    }
}
