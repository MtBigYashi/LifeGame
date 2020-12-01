using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGameViewModel.InteractionRequests
{
    public class UpdateGenerationRequest
    {
        public int Rank { get; set; }
        public Dictionary<int, List<bool>> GenerationInformation { get; set; }
    }
}
