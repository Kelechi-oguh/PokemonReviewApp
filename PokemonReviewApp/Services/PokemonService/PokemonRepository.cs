using PokemonReviewApp.Data;
using PokemonReviewApp.Models;
using PokemonReviewApp.Services.PokemonService;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;

        public PokemonRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemon.OrderBy(p => p.Id).ToList();
        }

        public Pokemon GetPokemonById(int id)
        {
            return _context.Pokemon.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemonByName(string name)
        {
            return _context.Pokemon.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int id)
        {
            var reviews = _context.Reviews.Where(p=> p.Pokemon.Id == id);
            if (reviews.Count() <= 0)
                return 0;
            var ratingAvg = (decimal)reviews.Sum(p => p.Rating)/reviews.Count();
            return Math.Round(ratingAvg, 2);
        }


        public bool PokemonExists(int id)
        {
            return _context.Pokemon.Any(p => p.Id == id);
        }

        public ICollection<int> GetAllPokemonsRatingById(int id)
        {
            var ratings = _context.Reviews.Where(r => r.Pokemon.Id == id).Select(r=> r.Rating).ToList();
            return ratings;
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(c=> c.Id == categoryId).FirstOrDefault();

            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon,
            };

            _context.PokemonOwners.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon,
            };

            _context.PokemonCategories.Add(pokemonCategory);
            
            _context.Pokemon.Add(pokemon);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePokemon(Pokemon pokemon)
        {
            _context.Pokemon.Update(pokemon);
            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Pokemon.Remove(pokemon);
            return Save();
        }
    }
}
