using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CarApp.Models;

namespace CarApp.Helper
{
    public class ToDataTable
    {
        public static DataTable ConvertToDataTable(List<CarDriveDtList> details)
        {
            var table = new DataTable();
            table.Columns.Add("DRDT_ID", typeof(string));
            table.Columns.Add("INVOICE_NO", typeof(string));
            table.Columns.Add("INVOICE_DT", typeof(DateTime));
            table.Columns.Add("INVOICE_AMT", typeof(decimal));
            table.Columns.Add("CAR_DR_TYPE", typeof(string));
            table.Columns.Add("NOTES", typeof(string));

            foreach (var item in details)
            {
                table.Rows.Add(
                    item.DRDT_ID,
                    item.INVOICE_NO,
                    item.INVOICE_DT,
                    item.INVOICE_AMT,
                    item.CAR_DR_TYPE,
                    item.NOTES
                );
            }

            return table;
        }

    }
}