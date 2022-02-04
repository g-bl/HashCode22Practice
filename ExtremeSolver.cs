using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HCode21PracticeRound
{
    class ExtremeSolver
    {
        private class Client
        {
            public int Id;
            public List<int> Likes = new List<int>();     // ingredient ids
            public List<int> Dislikes = new List<int>();
        }
        private class Ingredient
        {
            public int Id;
            public string Name;
            public List<int> Likes = new List<int>();     // client ids
            public List<int> Dislikes = new List<int>();
        }

        #region Utils
        private int _clientId = 0;
        private int _ingredientId = 0;
        private Dictionary<int, Client> _clientIds = new Dictionary<int, Client>();
        private Dictionary<int, Ingredient> _ingredientIds = new Dictionary<int, Ingredient>();
        private Dictionary<string, Ingredient> _ingredientNames = new Dictionary<string, Ingredient>();

        private void AddClientToList(List<Client> list, Client element)
        {
            element.Id = _clientId;
            list.Add(element);
            _clientIds.Add(element.Id, element); 
            _clientId++;
        }
        private void AddIngredientToList(List<Ingredient> list, Ingredient element)
        {
            element.Id = _ingredientId;
            list.Add(element);
            _ingredientIds.Add(element.Id, element);
            _ingredientNames.Add(element.Name, element);
            _ingredientId++;
        }
        private Client GetClientWithId(List<Client> list, int id)
        {
            return _clientIds[id];
        }
        private Ingredient GetIngredientWithId(List<Ingredient> list, int id)
        {
            return _ingredientIds[id];
        }
        private Ingredient GetIngredientWithName(List<Ingredient> list, string name)
        {
            return _ingredientNames[name];
        }
        private bool ContainsIngredientWithName(List<Ingredient> list, string name)
        {
            return _ingredientNames.ContainsKey(name);
        }
        #endregion

        public ExtremeSolver(string inputFileName, string outputFileName, char delimiter)
        {
            // Model initializations
            
            List<Client> clients = new();
            List<Ingredient> ingredients = new();

            /***************************************************************************
             * Input loading
             * *************************************************************************/

            Console.WriteLine($"Input loading... {inputFileName}");

            string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), inputFileName);
            string[] lines = File.ReadAllLines(inputFilePath);

            // Metadata parsing
            int nbPotentialClients = int.Parse(lines[0]);

            // Client likes and dislikes parsing
            for (int i = 1; i <= nbPotentialClients * 2; i += 2)
            {
                string[] likesLine = lines[i].Split(delimiter);
                string[] dislikesLine = lines[i+1].Split(delimiter);

                int nbL = int.Parse(likesLine[0]);
                int nbD = int.Parse(dislikesLine[0]);

                Client newClient = new();
                AddClientToList(clients, newClient);

                // Likes
                for (int l = 1; l <= nbL; l++)
                {
                    string ingredientName = likesLine[l];

                    if (!ContainsIngredientWithName(ingredients , ingredientName))
                    {
                        Ingredient newIngredient = new Ingredient();
                        newIngredient.Name = ingredientName;    
                        AddIngredientToList(ingredients, newIngredient);
                    }
                        
                    Ingredient ingredient = GetIngredientWithName(ingredients, ingredientName);

                    newClient.Likes.Add(ingredient.Id);
                    ingredient.Likes.Add(newClient.Id);
                }

                // Dislikes
                for (int d = 1; d <= nbD; d++)
                {
                    string ingredientName = dislikesLine[d];

                    if (!ContainsIngredientWithName(ingredients, ingredientName))
                    {
                        Ingredient newIngredient = new Ingredient();
                        newIngredient.Name = ingredientName;
                        AddIngredientToList(ingredients, newIngredient);
                    }

                    Ingredient ingredient = GetIngredientWithName(ingredients, ingredientName);

                    newClient.Dislikes.Add(ingredient.Id);
                    ingredient.Dislikes.Add(newClient.Id);
                }

            }

            /***************************************************************************
             * Extreme Solver
             * *************************************************************************/

            Dictionary<int, Ingredient> pizzaIngredients = new Dictionary<int, Ingredient>(); // <-- Fill this with the solution <id, Ingredient>

            // Compromise solver step #1 : sort ingredients by Likes - Dislikes
            ingredients = ingredients.OrderByDescending(ing => ing.Likes.Count - ing.Dislikes.Count).ToList();

            // Compromise solver step #2 : notify first and last ingredients as gload and trash
            Dictionary<int, Ingredient> goldIngredients = new Dictionary<int, Ingredient>();
            Dictionary<int, Ingredient> trashIngredients = new Dictionary<int, Ingredient>();
            for (int i = 0; i < ingredients.Count; i++)
            {
                if (i < ingredients.Count * 0.10)
                    goldIngredients.Add(ingredients[i].Id, ingredients[i]);
                if (i > ingredients.Count * 0.90)
                    trashIngredients.Add(ingredients[i].Id, ingredients[i]);
            }

            // Naive solver step #1 : adding all ingredients of value EXCEPT trash ingredients
            foreach (Client c in clients)
            {
                foreach (int l in c.Likes)
                {
                    if (!pizzaIngredients.ContainsKey(l) && !trashIngredients.ContainsKey(l))
                        pizzaIngredients.Add(l, GetIngredientWithId(ingredients, l));
                }
            }

            // Naive solver step #2 : removing disliked ingredients EXCEPT gold ingredients
            foreach (Client c in clients)
            {
                foreach (int d in c.Dislikes)
                {
                    if (pizzaIngredients.ContainsKey(d) && !goldIngredients.ContainsKey(d))
                        pizzaIngredients.Remove(d);
                }
            }

            /***************************************************************************
             * Output
             * *************************************************************************/

            Console.WriteLine($"Output to file... {inputFileName}");

            using (StreamWriter outputFile = new(Path.Combine(Directory.GetCurrentDirectory(), outputFileName)))
            {
                string output = "" + pizzaIngredients.Count();
                foreach (Ingredient ing in pizzaIngredients.Values)
                    output += " " + ing.Name;

                outputFile.WriteLine(output);
            }

            Console.WriteLine("Done.");
            Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), outputFileName));
        }
    }
}
