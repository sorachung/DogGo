using DogGo.Models;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IWalksRepository
    {
        List<Walks> GetAllWalks();
        //Walks GetWalkById();
        //List<Walks> GetWalksByWalkerId(int walkerId);
        void AddWalk(Walks walk);
        //void UpdateWalk(Walks walk);
        void DeleteWalk(int id);
        void DeleteWalksMultiple(List<int> ids);
    }
}
