using System;
using System.Collections.Generic;
using System.Data;
using TwoFunTwoMe_Common;
using TwoFunTwoMe_DataAccess;

namespace testProyecto
{
    class Program
    {
        static void Main(string[] args)
        {
            //var dto = new DynamicDto
            //{    new SpParameter
            //        {
            //            Name = "INT_SUB_ORIGEN",
            //            Value = xInfoClass.INT_SUB_ORIGEN.ToString()
            //        },
            //    SPName = "usp_ConsultarAgentes"
            //};
            var dto = new DynamicDto
            {
                ParameterList = new List<SpParameter>
                {
                    
                },
                Result = null,
                SPName = "usp_ConsultarAgentes"
            };

            dto.Result = null;

            DynamicDto ds = DynamicSqlDAO.ExecuterSp(dto, "Data Source=192.168.110.15 ;Initial Catalog=FinTech_DEV;Persist Security Info=True;User ID=icortes;Password=Ics5355.;");

            var res = ds.Result.Tables[0];
            foreach (DataRow row in res.Rows)
            {
                var data = row[0];
            }
        }
    }
}
