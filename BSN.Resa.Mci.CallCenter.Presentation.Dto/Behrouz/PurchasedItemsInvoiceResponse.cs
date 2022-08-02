namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Behrouz
{
    public class PurchasesInfo
    {
        public double PURCHASE_ID { get; set; }

        public string PURCHASE_DATE { get; set; }

        public double CHICKENBOUGHT_CASH { get; set; } // پرداخت نقدی

        public double CHICKENBOUGHT_SUBSIDY { get; set; } // پرداخت از یارانه

        public double CHICKENBOUGHT_WALLET { get; set; } // میزان پرداخت کیف پول

        public double EGGSBOUGHT_CASH { get; set; } // پرداخت نقدی

        public double EGGSBOUGHT_SUBSIDY { get; set; } // پرداخت از یارانه

        public double EGGSBOUGHT_WALLET { get; set; } // میزان پرداخت کیف پول

        public double OILBOUGHT_CASH { get; set; } // پرداخت نقدی

        public double OILBOUGHT_SUBSIDY { get; set; } // پرداخت از یارانه

        public double OILBOUGHT_WALLET { get; set; } // میزان پرداخت کیف پول

        public double DAIRYBOUGHT_CASH { get; set; } // پرداخت نقدی

        public double DAIRYBOUGHT_SUBSIDY { get; set; } // پرداخت از یارانه

        public double DAIRYBOUGHT_WALLET { get; set; } // میزان پرداخت کیف پول

        public double BREADBOUGHT_CASH { get; set; } // پرداخت نقدی

        public double BREADBOUGHT_SUBSIDY { get; set; } // پرداخت از یارانه

        public double BREADBOUGHT_WALLET { get; set; } // میزان پرداخت کیف پول

        public double FLOURBOUGHT_CASH { get; set; } // پرداخت نقدی

        public double FLOURBOUGHT_SUBSIDY { get; set; } // پرداخت از یارانه

        public double FLOURBOUGHT_WALLET { get; set; } // میزان پرداخت کیف پول

        public double TOTAL { get; set; } // میزان پرداخت کیف پول

        public double STOREID { get; set; } // کد فروشگاه

        public double STORE_NATIONALCODE { get; set; } // کد ملی فروشگاه

        public string STORENAME { get; set; } // نام فروشگاه
    }
}
