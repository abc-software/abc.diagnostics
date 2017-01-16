using System.Collections.Generic;
using Diagnostic.ExtraInformation;

namespace EnoughPI.Logging
{
    public class ExtraInfoProvider : IExtraInformationProvider
    {
        private string value;
        private int digits;

        public ExtraInfoProvider(string value, int digits)
        {
            this.value = value;
            this.digits = digits;
        }

        public void PopulateDictionary(IDictionary<string, object> dictionary)
        {
            if (this.value != null)
            {
                dictionary.Add("PI", this.value);
                dictionary.Add("Digits", this.digits);
            }
        }
    }
}
