using AutoMapper;
using Business.DTOs;
using DataAccess;

namespace Business.Mappers
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<User, UserDTO>().ReverseMap();
      CreateMap<RegisterModelDTO, User>().ReverseMap();
      ////////////////////////////////////////////////
      CreateMap<TestParentTable, TestDTO>()
        .ForMember(x => x.Childs, opt => opt.MapFrom(ChildsList));
      CreateMap<TestDTO, TestParentTable>()
        .ForMember(x => x.Childs, opt => opt.MapFrom(ChildsClass));
    }

    private List<double> ChildsList(TestParentTable testParent, TestDTO testDTO)
    {
      var resultado = new List<double>();
      if (testParent.Childs == null) { return resultado; }
      foreach (var child in testParent.Childs)
      {
        resultado.Add(child.Doublevalue);
      }
      return resultado;
    }
    private List<TestChildTable> ChildsClass(TestDTO testDTO, TestParentTable testParent)
    {
      var resultado = new List<TestChildTable>();
      if (testDTO.Childs == null) { return resultado; }
      foreach (var child in testDTO.Childs)
      {
        resultado.Add(
          new(){
            IdParent = testDTO.Id,
            Doublevalue = child
          }
        );
      }
      return resultado;
    }
  }
}