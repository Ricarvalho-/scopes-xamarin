using System;
using Newtonsoft.Json;

namespace Scopes.Model
{
    public class Goal
    {
        public string Title { get; set; }
        public int Progress { get; set; }

        [JsonIgnore] public bool IsToDo => Progress == 0;
        [JsonIgnore] public bool IsDoing => Progress == 1;
        [JsonIgnore] public bool IsDone => Progress == 2;

        [JsonIgnore] public bool CanCancel => IsDoing || IsDone;
        [JsonIgnore] public bool CanBegin => IsToDo;
        [JsonIgnore] public bool CanFinish => IsDoing;

        [JsonIgnore] public string Status => StatusFromProgress();

        public void Cancel() => Progress = 0;
        public void Begin() => Progress = 1;
        public void Finish() => Progress = 2;

        private string StatusFromProgress()
        {
            switch(Progress)
            {
                case 0:
                    return "To Do";
                case 1:
                    return "Doing";
                case 2:
                    return "Done";
                default:
                    return "Unknown";
            }
        }
    }
}
