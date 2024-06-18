namespace SecretShare.Models.ViewModel
{
    public class UploadFileVm
    {

        public string Id { get; set; }
        public string UserId { get; set; }
        
        public bool AutoDelete { get; set; }
        public DateTime UploadDate { get; set; }
        public bool HasbeenDowloaded { get; set; }
        public bool PublicFile { get; set; } = true;

    }
}
