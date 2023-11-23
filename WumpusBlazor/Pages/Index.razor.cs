using Blazored.Modal;
using Blazored.Modal.Services;
using Lea;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using WumpusBlazor.Components;
using WumpusEngine;
using WumpusEngine.Events;
using static Lea.IEventAggregator;

namespace WumpusBlazor.Pages
{
    public partial class Index
    {
        private EventAggregatorHandler<NewGameStarted> _onNewGameStarted = default!;
        private AsyncEventAggregatorHandler<GameStateChanged> _onGameStateChange = default!;
        private AsyncEventAggregatorHandler<BatMoved> _onBatMovedChange = default!;

        private static IModalReference? _fireModal;
        private static IModalReference? _batModal;
        private static IModalReference? _gameOverModal;

        // Is there a better way of bridging this js event gap with HandleKeyDown needing to be static for discovery?
        private static Engine? _engine = null;
        [Inject]
#pragma warning disable CA1822 // Mark members as static
        public Engine Engine
#pragma warning restore CA1822 // Mark members as static
        {
            get => _engine!;
            set => _engine = value;
        }

        [Inject]
        public IEventAggregator EventAggregator { get; set; } = default!;

        [CascadingParameter]
        public IModalService ModalService { get; set; } = default!;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _onNewGameStarted = NewGameStartedHandler;
                _onGameStateChange = GameStateChangeHandler;
                _onBatMovedChange = BatMovedChangeHandler;

                EventAggregator.Subscribe(_onNewGameStarted);
                EventAggregator.Subscribe(_onGameStateChange);
                EventAggregator.Subscribe(_onBatMovedChange);
                await jsRuntime.InvokeVoidAsync("JsFunctions.addKeyboardListenerEvent");
            }
        }

        private void NewGameStartedHandler(NewGameStarted state)
        {
            StateHasChanged();
        }

        [JSInvokable("HandleKeyDown")]
        public static async Task HandleKeyDown(KeyboardEventArgs args)
        {
            if ((_batModal != null || _gameOverModal != null) && args.Key == "Escape")
            {
                await CloseModal();
            }
            else
            {
                _engine?.HandleKeyboardEvent(args.Key);
            }
        }

        private async Task BatMovedChangeHandler(BatMoved evt)
        {
            var parameters = new ModalParameters()
                .Add("BatMoved", evt);

            _batModal = ModalService.Show<BatModal>("Bat!", parameters, _modalOptions);
            _ = await _batModal.Result;
            _batModal = null;
        }

        private async Task GameStateChangeHandler(GameStateChanged evt)
        {
            await CloseModal();

            switch (evt.NewGameState)
            {
                case GameState.Firing:
                    _fireModal = ModalService.Show<FireModal>("Attack!", _modalOptions);
                    var fireResult = await _fireModal.Result;

                    if (fireResult.Cancelled && Engine!.GameState == GameState.Firing)
                    {
                        Engine.TriggerFireMode();
                    }

                    _fireModal = null;
                    break;
                case GameState.Pit:
                case GameState.Missed:
                case GameState.Eaten:
                case GameState.Won:
                    var parameters = new ModalParameters()
                        //.Add("Engine", Engine!);
                        .Add("Message", Engine!.EndGameMessage)
                        .Add("GameState", Engine!.GameState);
                    _gameOverModal = ModalService.Show<GameOverModal>("Game Over", parameters, _modalOptions);
                    var goResult = await _gameOverModal.Result;
                    _gameOverModal = null;                    
                    if (goResult.Confirmed)
                    {
                        Engine.StartNewGame();
                    }
                    break;
                default:
                    break;
            }
        }

        private readonly ModalOptions _modalOptions = new()
        {
            Position = ModalPosition.Middle,
            ActivateFocusTrap = true,
            DisableBackgroundCancel = true,
            UseCustomLayout = true
        };

        private static async Task CloseModal()
        {
            if (_fireModal != null)
            {
                _fireModal.Close(ModalResult.Cancel());
                var result = await _fireModal.Result;

                if (result.Cancelled && _engine!.GameState == GameState.Firing)
                {
                    _engine.TriggerFireMode();
                }
                _fireModal = null;
            }

            if (_batModal != null)
            {
                _batModal.Close(ModalResult.Cancel());
                _ = await _batModal.Result;
                _batModal = null;
            }

            if (_gameOverModal != null)
            {
                _gameOverModal.Close(ModalResult.Cancel());
                _ = await _gameOverModal.Result;
                _gameOverModal = null;
            }
        }
    }
}
