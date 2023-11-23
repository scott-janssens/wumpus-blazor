using Lea;
using Microsoft.AspNetCore.Components;
using WumpusBlazor.Helpers;
using WumpusEngine;
using WumpusEngine.Events;
using static Lea.IEventAggregator;

namespace WumpusBlazor.Components
{
    public partial class Cell
    {
        private EventAggregatorHandler<CavernUpdated> _onCavernUpdated = default!;
        private string _cellColor = "brown";

        [Inject]
        public IEventAggregator EventAggregator { get; set; } = default!;

        [Inject]
        public ISvgHelper SvgHelper { get; set; } = default!;

        [Parameter]
        public Cavern Cavern { get; set; } = default!;

        [Parameter]
        public uint Size { get; set; } = 150;

        [Parameter]
        public string Background { get; set; } = "white";

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            _cellColor = Cavern.IsAdjacentPit ? "greenyellow" : Cavern.IsPit ? "green" : "brown";

            //Cavern.Reveal();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                _onCavernUpdated = CavernUpdatedHander;
                EventAggregator.Subscribe(_onCavernUpdated);
            }
        }

        private void CavernUpdatedHander(CavernUpdated e)
        {
            if (ReferenceEquals(e.Cavern, Cavern))
            {
                StateHasChanged();
            }
        }
    }
}
