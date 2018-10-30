namespace TwoFunTwoMe_Common
{
    public class DynamicDto
    {
        public DynamicDto()
        {
            this.TypeSP = "json";
        }

        public string SPName { get; set; }

        public string TypeSP { get; set; }

        public string SqlQuery { get; set; }

        public System.Data.DataSet Result { get; set; }

        public System.Collections.Generic.List<SpParameter> ParameterList { get; set; }

        public bool HasResult
        {
            get
            {
                if (this.Result != null && this.Result.Tables.Count > 0)
                    return this.Result.Tables[0].Rows.Count > 0;
                return false;
            }
        }

        public void Dispose()
        {
            this.ParameterList = (System.Collections.Generic.List<SpParameter>)null;
            this.Result = (System.Data.DataSet)null;
        }

        public string EmailsRecipient { get; set; }

        public string EmailsHiddenRecipient { get; set; }
    }
}
