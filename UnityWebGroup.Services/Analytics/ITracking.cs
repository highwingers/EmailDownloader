namespace UnityWebGroup.Services.Analytics
{
    public interface ITracking
    {
        void TrackPage(string page, string title);
    }
}