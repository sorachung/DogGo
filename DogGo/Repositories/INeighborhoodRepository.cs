using DogGo.Models;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface INeighborhoodRepository
    {
        List<Neighborhood> GetAll();
    }
}