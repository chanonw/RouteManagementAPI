using System.Collections.Generic;
using System.Text;
using RouteAPI.Models;

namespace RouteAPI.Helpers
{
    public static class TemplateGenerator
    {
        public static string GetHTMLString(List<Delivery> reportDataList)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                <html>
                <head>
                </head>
                <body>
                <div class='header'><h1>This is the generated PDF report!!!</h1></div>
                <table align='center'>
                <tr>
                    <th>No</th>
                    <th>Firstname</th>
                    <th>Lastname</th>
                    <th>Quantity</th>
                    <th>ไปส่งได้</th>
                    <th>คืนถัง</th>
                    <th>คูปอง</th>
                </tr>");
            for (int i = 0; i < reportDataList.Count; i++)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                    <td>{4}</td>
                                    <td>{5}</td>
                                    <td>{6}</td>
                                  </tr>", i + 1, reportDataList[i].Customer.firstName, reportDataList[i].Customer.lastName, 
                                  reportDataList[i].quantity, ".....", ".....", ".....");
            }
            sb.Append(@"
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }
    }
}