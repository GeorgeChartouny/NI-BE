namespace NI_BE.Watcher
{
    public class OnRenamedEvent
    {
        public void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine(e.OldName + " is changed to " + e.Name);
        }
    }
}
