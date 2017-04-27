using AutoMapper;
using System.Collections.Generic;

namespace GenericApi
{
    public static class DTOExtensions
    {
        public static DTO AsViewModel<DTO, Model>(this Model model)
        {
            return Mapper.Map<Model, DTO>(model);
        }

        public static Model AsModel<DTO, Model>(this DTO dto)
        {
            return Mapper.Map<DTO, Model>(dto);
        }

        public static Model AsModel<DTO, Model>(this DTO dto, Model model)
        {
            return Mapper.Map<DTO, Model>(dto, model);
        }

        public static List<DTO> AsViewModel<DTO, Model>(this List<Model> models)
        {
            var list = new List<DTO>();
            models.ForEach(f => list.Add(AsViewModel<DTO, Model>(f)));
            return list;
        }

        public static List<Model> AsModel<DTO, Model>(this List<DTO> models)
        {
            var list = new List<Model>();
            models.ForEach(f => list.Add(AsModel<DTO, Model>(f)));
            return list;
        }
    }
}
