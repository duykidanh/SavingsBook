using AutoMapper;
using SavingsBook.Application.Contracts.Common;
using SavingsBook.Application.Contracts.SavingBook;
using SavingsBook.Application.Contracts.SavingBook.Dto;
using SavingsBook.Application.Contracts.SavingRegulation;
using SavingsBook.Application.Contracts.SavingRegulation.Dto;

namespace SavingsBook.Application.AutoMapperProfile;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        #region Saving regulations

        CreateMap<CreateUpdateRegulationDto, Regulation>();
        CreateMap<CreateUpdateRegulationDto.SavingType, Regulation.SavingType>();

        CreateMap<Regulation, RegulationDto>();
        CreateMap<Regulation.SavingType, RegulationDto.SavingType>();

        #endregion

        #region Saving book

        CreateMap<CreateUpdateSavingBookDto, SavingBook>();

        CreateMap<SavingBook, SavingBookDto>();
        CreateMap<SavingBook.Regulation, SavingBookDto.Regulation>();


        #endregion

        CreateMap<Address, Application.Contracts.Common.Address>().ReverseMap();



    }
}