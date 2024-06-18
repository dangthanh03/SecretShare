namespace SecretShare.Models.ViewModel
{
    public class UploadTextVm
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public bool HasbeenDowloaded { get; set; }
        public bool AutoDelete { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
