using PokemonReviewApp.Data;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Services.CountryService
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context)
        {
            _context = context;
        }
        public bool CountryExists(int countryId)
        {
            return _context.Countries.Any(c=> c.Id == countryId);
        }

        public ICollection<Country> GetCountries()
        {
            return _context.Countries.ToList();
        }

        public Country GetCountry(int countryId)
        {
            return _context.Countries.FirstOrDefault(c=> c.Id == countryId);
        }

        public Country GetCountryByOwner(int ownerId)
        {
            return _context.Owners.Where(o=> o.Id == ownerId).Select(o=> o.Country).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnersFromCountry(int countryId)
        {
            return _context.Owners.Where(o=> o.Country.Id == countryId).ToList();
        }

        public bool Save()
        {
            var saved  = _context.SaveChanges();
            return saved > 0? true: false;
        }

        public bool CreateCountry(Country country)
        {
            _context.Countries.Add(country);
            return Save();
        }

        public bool UpdateCountry(Country country)
        {
            _context.Countries.Update(country);
            return Save();
        }

        public bool DeleteCountry(Country country)
        {
            _context.Countries.Remove(country);
            return Save();
        }
    }
}
