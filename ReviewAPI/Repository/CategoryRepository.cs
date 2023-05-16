using ReviewAPI.Data;
using ReviewAPI.Interface;
using ReviewAPI.Models;

namespace ReviewAPI.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public readonly DataContext _context;

        public CategoryRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Where(e => e.Id == id).FirstOrDefault();
        }

        public ICollection<Pokemon> GetPokemonByCategory(int categoryId)
        {
            return _context.PokemonCategories.Where(e => e.CategoryId == categoryId).Select(c=>c.Pokemon).ToList();
        }        
        
        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }
    }
}
