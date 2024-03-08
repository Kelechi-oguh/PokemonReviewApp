using AutoMapper;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //MAPPING FOR GET METHODS
            CreateMap<Category, CategoryDto>();
            CreateMap<Pokemon, PokemonDto>();
            CreateMap<Country, CountryDto>();
            CreateMap<Owner, OwnerDto>();
            CreateMap<Review, ReviewDto>();
            CreateMap<Reviewer, ReviewerDto>();

            //MAPPING FOR POST METHODS
            CreateMap<CategoryDto, Category>();
            CreateMap<CountryDto, Country>();
            CreateMap<OwnerDto, Owner>();
            CreateMap<PokemonDto, Pokemon>();
            CreateMap<ReviewDto, Review>();
            CreateMap<ReviewerPostDto, Reviewer>();
        }
    }
}
