namespace API.ViewModels
{
    public class SettingViewModel
    {
        public DateForm DFormat { get; set; }
    }
    public enum DateForm
    {
        YYYYMMDD,
        DDMMYYYY,
        MMDDYYYY
    }
}
