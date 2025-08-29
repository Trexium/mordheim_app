
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;


namespace HexedSceneryMobileApp.Services
{
    public interface IDiceService
    {
        Task<Models.DiceType> GetDiceTypeAsync(int diceTypeId);
        Task LoadCachesAsync();
        Task<bool> ValidateDiceResult(int result, int numberOfDice, int diceTypeId);
    }
    public class DiceService : IDiceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private static Dictionary<int, Models.DiceType> _diceTypeCache = new Dictionary<int, Models.DiceType>();
        private static Dictionary<int, int[]> _diceTypeValidResults = new Dictionary<int, int[]>();

        public DiceService(IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
        }

        public async Task<Models.DiceType> GetDiceTypeAsync(int diceTypeId)
        {
            if (!_diceTypeCache.ContainsKey(diceTypeId))
            {
                await LoadCachesAsync();
            }

            return _diceTypeCache[diceTypeId];
        }

        public async Task<bool> ValidateDiceResult(int result, int numberOfDice, int diceTypeId)
        {
            if (!_diceTypeValidResults.TryGetValue(diceTypeId, out var validResults))
            {
                AddValidResultsToCache(diceTypeId);
                if (!_diceTypeValidResults.TryGetValue(diceTypeId, out validResults))
                {
                    return false;
                }
            }
            
            return validResults.Contains(result / numberOfDice);
        }

        public async Task LoadCachesAsync()
        {
            var url = $"dice";

            if (_diceTypeCache.Count == 0)
            {
                try
                {
                    using (var httpClient = _httpClientFactory.CreateClient("HexedApi"))
                    {
                        var data = await httpClient.GetFromJsonAsync<List<ApiModels.DiceType>>(url);
                        var diceTypes = _mapper.Map<List<Models.DiceType>>(data);

                        foreach(var diceType in diceTypes)
                        {
                            _diceTypeCache.Add(diceType.Id, diceType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void AddValidResultsToCache(int diceTypeId)
        {
            var validResults = new List<int>();
            switch ((Enums.Die)diceTypeId)
            {
                case Enums.Die.D2:
                    validResults.AddRange(new []{ 1, 2 });
                    break;
                case Enums.Die.D3:
                    validResults.AddRange(new []{ 1, 2, 3 });
                    break;
                case Enums.Die.D4:
                    validResults.AddRange(new []{ 1, 2, 3, 4 });
                    break;
                case Enums.Die.D6:
                    validResults.AddRange(new []{ 1, 2, 3, 4, 5, 6 });
                    break;
                case Enums.Die.D8:
                    validResults.AddRange(new []{ 1, 2, 3, 4, 5, 6, 7, 8 });
                    break;
                case Enums.Die.D10:
                    validResults.AddRange(new []{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
                    break;
                case Enums.Die.D12:
                    validResults.AddRange(new []{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
                    break;
                case Enums.Die.D20:
                    validResults.AddRange(new []{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
                    break;
                case Enums.Die.D66:
                    for (int i = 11; i <= 66; i++)
                    {
                        var tens = i / 10;
                        var units = i % 10;
                        if (tens >= 1 && tens <= 6 && units >= 1 && units <= 6)
                        {
                            validResults.Add(i);
                        }
                    }
                    break;
                case Enums.Die.D100:
                    validResults.AddRange(Enumerable.Range(1, 100));
                    break;
            }

            _diceTypeValidResults[diceTypeId] = validResults.ToArray();
        }
    }
}
