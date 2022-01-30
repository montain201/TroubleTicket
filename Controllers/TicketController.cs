using Core.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public TicketController(IConfiguration configuration,IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _env = environment;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"SELECT TOP(1) T.TicketId,TS.TicketStatusId,TicketNo,T.TicketType,TicketState,TicketStatusDescription,TS.CreationDate,AttachmentPath
	                                    FROM Ticket T LEFT JOIN TicketStatus TS ON T.TicketId = TS.TicketId
				                                      LEFT JOIN TicketAttachment TA ON TS.TicketStatusId = TA.TicketStatusId
                                        WHERE T.TicketId = 1 ORDER BY TS.TicketStatusId DESC";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("SQLServerConnection");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            //foreach (DataRow row in table.Rows)
            //{
            //    int val = (int) row["TicketState"];
            //    var name = Enum.GetName(typeof(TicketState), row["TicketState"]);
            //    row["TicketState"] = name;
            //}

            return new JsonResult(table);
        }
    }
}
