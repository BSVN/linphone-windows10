namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Rubika
{
    public class User
    {
        public string first_name { get; set; }

        public string last_name { get; set; }

        public string national_id { get; set; }

        public string birth_date { get; set; }

        public bool is_head { get; set; }
    }

    public class UserInfo
    {
        public User User { get; set; }
    }
}
