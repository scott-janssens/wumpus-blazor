using Lea;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WumpusEngine;

namespace WumpusBlazor.Shared
{
    public partial class NavMenu
    {
        [Inject]
        public IEventAggregator Ea { get; set; } = default!;

        [Parameter]
        public Engine Engine { get; set; } = default!;

        public string Difficulty { get; private set; } = ((uint)GameDifficulty.Normal).ToString();

        private Cavern _pitWarnCavern = default!;
        private Cavern _pitCavern = default!;
        private Cavern _bloodCavern = default!;
        private ElementReference newGameBtn = default!;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _pitWarnCavern = new Cavern(Ea, new Location(0, 0))
            {
                IsAdjacentPit = true
            };
            _pitWarnCavern.North = _pitWarnCavern.East = _pitWarnCavern.South = _pitWarnCavern.West = _pitWarnCavern;
            _pitWarnCavern.Reveal();

            _pitCavern = new Cavern(Ea, new Location(0, 0))
            {
                IsPit = true
            };
            _pitCavern.North = _pitCavern.East = _pitCavern.South = _pitCavern.West = _pitCavern;
            _pitCavern.Reveal();

            _bloodCavern = new Cavern(Ea, new Location(0, 0))
            {
                HasBlood = true
            };
            _bloodCavern.North = _bloodCavern.East = _bloodCavern.South = _bloodCavern.West = _bloodCavern;
            _bloodCavern.Reveal();
        }

        private void SetDifficulty(ChangeEventArgs e)
        {
            Difficulty = (string)e.Value!;
        }

        private async Task NewGame()
        {
            Engine.StartNewGame(DifficultyOptions.FromGameDifficulty((GameDifficulty)uint.Parse(Difficulty)), new RandomHelper());
            await jsRuntime.InvokeVoidAsync("ElementFunctions.blurElement", newGameBtn);
        }
    }
}
