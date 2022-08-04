namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Rubika
{
    public class GenericRubikaServiceResonseBase<T> where T : class
    {
        public string Status { get; set; }

        public T Data { get; set; }

        public bool IsSuccess => Status?.ToUpper() == "OK";
    }
}
