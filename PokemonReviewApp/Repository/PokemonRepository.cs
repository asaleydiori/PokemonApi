using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository:IPokemonRepository
    {
        private readonly DataContext _dataContext;
        public PokemonRepository(DataContext context) 
        {
            _dataContext = context;
        }

        //Create Pokemon
        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _dataContext.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
            var category = _dataContext.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon
            };
            _dataContext.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon
            };
            _dataContext.Add(pokemonCategory);
            _dataContext.Add(pokemon);

            return Save();
        }

        public Pokemon GetPokemonId(int id)
        {
            return _dataContext.Pokemon.Where(p => p.Id ==id).FirstOrDefault();
        }

        public Pokemon GetPokemonName(string name)
        {
            return _dataContext.Pokemon.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int id)
        {
            var review = _dataContext.Reviews.Where(p => p.Id == id);
            if (review.Count() <= 0)
                return 0;
            return ((decimal)review.Sum(p => p.Rating)/review.Count());
                      
           
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _dataContext.Pokemon.OrderBy(p => p.Id).ToList();
        }

        public bool PokemonExist(int id)
        {
            return _dataContext.Pokemon.Any(p => p.Id == id);
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

       
    }
}
