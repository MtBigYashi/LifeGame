using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGameViewModel.InteractionRequests
{
    public class ToggleCurrentCellAliveRequest
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
