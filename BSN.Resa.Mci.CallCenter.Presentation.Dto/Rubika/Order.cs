using System.Collections.Generic;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Rubika
{
    public class Order
    {
        public string store_code { get; set; }
        public string store_name { get; set; }
        public string store_phone { get; set; }
        public string store_address { get; set; }
        public string store_city_name { get; set; }
        public string store_province_name { get; set; }
        public string order_track_id { get; set; }
        public List<Product> products { get; set; }
        public List<Product> used_items { get; set; }
        public int order_time { get; set; }
        public int total_price { get; set; }
    }

}
