using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IWalkerRepository
    {
        List<Walker> GetAllWalkers();
        Walker GetWalkerById(int id);
        void AddWalker(Walker walker);
        void UpdateWalker(Walker walker);
        public void DeleteWalker(int id);
    }
}