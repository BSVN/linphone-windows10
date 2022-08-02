namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Behrouz
{
    public class ChainStoreInfo
    {
        public string TrackID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Village { get; set; }
        public string Address { get; set; }
        public string Guild_Number { get; set; }
        public string OwnerName { get; set; }
        public string OwnerFamily { get; set; }
        public string OwnerNationalID { get; set; }
        public string OwnerMobile { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public string StoreEmployees { get; set; }
        public string ShebaNumber { get; set; }
        public string Categories { get; set; }
        public string[] SplitedCategories => Categories == "_" ? new string[] { } : Categories.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }
}
