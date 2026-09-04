namespace Tools.SaveSystem
{
    public interface ISaveable
    {
        public object CaptureState();
        public void RestoreState(object state);
        public string GetUniqueId();
    } 
}