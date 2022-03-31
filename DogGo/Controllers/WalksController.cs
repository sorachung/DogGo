using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DogGo.Repositories;
using DogGo.Models;
using DogGo.Models.ViewModels;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace DogGo.Controllers
{
    public class WalksController : Controller
    {
        private readonly IWalksRepository _walksRepo;
        private readonly IWalkerRepository _walkerRepo;
        private readonly IDogRepository _dogRepo;

        public WalksController(IWalksRepository walksRepository, IWalkerRepository walkerRepository, IDogRepository dogRepository)
        {
            _walksRepo = walksRepository;
            _walkerRepo = walkerRepository;
            _dogRepo = dogRepository;

        }
        // GET: WalksController
        public ActionResult Index()
        {
            List<Walks> walks = _walksRepo.GetAllWalks();
            return View(walks);
        }

        // GET: WalksController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: WalksController/Create
        public ActionResult Create()
        {
            List<Walker> walkers = _walkerRepo.GetAllWalkers();
            List<Dog> dogs = _dogRepo.GetAllDogs();

            WalksFormViewModel vm = new WalksFormViewModel()
            {
                Walk = new Walks(),
                Dogs = dogs,
                Walkers = walkers
            };

            return View(vm);
        }

        // POST: WalksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WalksFormViewModel vm)
        {
            try
            {
                foreach (var dogId in vm.SelectedDogIds)
                {
                    vm.Walk.DogId = dogId;
                    _walksRepo.AddWalk(vm.Walk);
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(vm);
            }
        }

        // GET: WalksController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalksController/Delete/5
        public ActionResult Delete(int id)
        {
            Walks walk = _walksRepo.GetWalkById(id);
            return View(walk);
        }

        // POST: WalksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Walks walk)
        {
            try
            {
                _walksRepo.DeleteWalk(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(walk);
            }
        }

        // POST: WalksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMultiple(Dictionary<string, int> ids)
        {
            try
            {
                var selectedIds = ids.Values.ToList();
                selectedIds.RemoveAt(selectedIds.Count - 1);
                _walksRepo.DeleteWalksMultiple(selectedIds);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
