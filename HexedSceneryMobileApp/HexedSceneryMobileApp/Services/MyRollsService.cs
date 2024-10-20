﻿using AutoMapper;
using HexedSceneryMobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedSceneryMobileApp.Services
{
    public interface IMyRollsService
    {
        Task<List<Roll>> GetMyRolls();
        Task<Guid> AddRoll(Roll roll);
        Task AddChildRoll(Guid parentId, DiceResult childResult);
        Task RemoveRoll(Guid id);
        Task RemoveRoll(Roll roll);
        List<Roll> MyRolls { get; }
        Task RemoveAll();
        Task<Roll> GetRoll(Guid id);
    }
    public class MyRollsService : IMyRollsService
    {
        private readonly IMapper _mapper;
        private readonly IEncounterService _encounterService;
        private readonly IDiceChartService _diceChartService;

        private static Dictionary<Guid, Roll> _rollsCache = new Dictionary<Guid, Roll>();

        public List<Roll> MyRolls => _rollsCache.Values.ToList();

        public MyRollsService(IMapper mapper, IEncounterService encounterService, IDiceChartService diceChartService)
        {
            _mapper = mapper;
            _encounterService = encounterService;
            _diceChartService = diceChartService;
        }

        public async Task<Guid> AddRoll(Roll roll)
        {
            var uniqueId = Guid.NewGuid();
            roll.Id = uniqueId;
            
            _rollsCache.Add(roll.Id, roll);
            return roll.Id;
        }


        // Not complete, probobly has to be rewritten
        public async Task AddChildRoll(Guid parentId, DiceResult childResult)
        {
            if (_rollsCache[parentId].ChildChartResults == null)
            {
                _rollsCache[parentId].ChildChartResults = new List<DiceResult>();
            }
            _rollsCache[parentId].ChildChartResults.Add(childResult);
        }

        public async Task<List<Roll>> GetMyRolls()
        {
            return _rollsCache.Values.ToList();
        }

        public async Task RemoveRoll(Guid id)
        {
            if (_rollsCache.ContainsKey(id))
            {
                _rollsCache.Remove(id);
            }
        }

        public async Task RemoveRoll(Roll roll)
        {
            await RemoveRoll(roll.Id);
        }

        public async Task RemoveAll()
        {
            _rollsCache.Clear();
        }

        public async Task<Roll> GetRoll(Guid id)
        {
            if (_rollsCache.TryGetValue(id, out var roll))
            {
                return roll;
            }
            return null;
        }
    }
}
