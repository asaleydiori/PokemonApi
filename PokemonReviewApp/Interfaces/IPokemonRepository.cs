using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemonId(int id);
        Pokemon GetPokemonName(string name);
        decimal GetPokemonRating(int id);
        bool PokemonExist(int id);
        bool CreatePokemon(int ownerId,int categoryId,Pokemon pokemon);
        bool Save();
    }
}
