using Ahmed_mart.Dtos.v1.StudentDto;
using Ahmed_mart.Models.v1.MONGODB;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, GetStudentDto>();
            CreateMap<AddStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();
        }
    }
}
