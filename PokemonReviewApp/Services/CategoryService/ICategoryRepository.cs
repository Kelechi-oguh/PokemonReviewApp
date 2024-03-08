using PokemonReviewApp.Models;

namespace PokemonReviewApp.Services.CategoryService
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        ICollection<Pokemon> GetPokemonByCategory(int categoryId);
        bool CategoriesExists(int categoryId);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}
