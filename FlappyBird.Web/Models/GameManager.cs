using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FlappyBird.Web.Models
{
    public class GameManager : INotifyPropertyChanged
    {
        private readonly int _gravity = 2;
        private readonly int _speed = 5;
        public event PropertyChangedEventHandler PropertyChanged;
        public BirdModel Bird { get; private set; }
        public ObservableCollection<PipeModel> Pipes { get; private set; }
        public GameManager()
        {
            
            Bird = new BirdModel();
            Pipes = new ObservableCollection<PipeModel>();
            Pipes.CollectionChanged += (o, e) => 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pipes)));
        }
        public async void MainLoop()
        {
            IsRunning = true;
            while (IsRunning)
            {
                Bird.Fall(_gravity);
                foreach (var pipe in Pipes)
                {
                    pipe.Move(_speed);
                }
                if (Bird.DistanceFromBottom <= 0)
                    GameOver();
                if (!Pipes.Any() || Pipes.Last().DistanceFromLeft < 250)
                    GeneratePipe();
                if (Pipes.First().DistanceFromLeft < -60)
                    Pipes.Remove(Pipes.First());
                var centeredPipe = GetCenteredPipe();
                if (centeredPipe != null)
                {
                    // pipe height - ground height + pipe distance from bottom
                    var min = 300 - 150 + centeredPipe.DistanceFromBottom;
                    // pipe gap - ground height + pipe distance from bottom - height of bird
                    var max = 430 - 150 + centeredPipe.DistanceFromBottom - 45;
                    if (Bird.DistanceFromBottom < min || Bird.DistanceFromBottom > max)
                        GameOver();
                }
                await Task.Delay(20);
            }
        }

        public bool IsRunning { get; private set; } = false;

        private void GameOver()
        {
            IsRunning = false;
        }

        private void GeneratePipe()
        {
            Pipes.Add(new PipeModel());
        }

        private PipeModel GetCenteredPipe()
        {
            return Pipes.FirstOrDefault(p => p.IsCentered());
        }


    }
}
