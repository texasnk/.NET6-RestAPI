using ReviewAPI.Models;

namespace ReviewAPI.Interface
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
    }
}
