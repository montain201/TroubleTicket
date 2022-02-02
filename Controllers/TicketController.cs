using Core.Data;
using Core.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly AuthDbContext _authDbContext;

        public TicketController(IConfiguration configuration,IWebHostEnvironment environment,AuthDbContext authDbContext)
        {
            _configuration = configuration;
            _env = environment;
            _authDbContext = authDbContext;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @" WITH LastTicketStatus AS (
                              SELECT TS.*,T.TicketNo,T.TicketType, ROW_NUMBER() OVER (PARTITION BY TS.ticketid ORDER BY creationdate DESC) AS RowNo
                              FROM TicketStatus AS TS INNER JOIN Ticket T ON T.TicketId = TS.TicketId 
                            )
                            SELECT * FROM LastTicketStatus LS WHERE LS.RowNo = 1";

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

        [HttpPost]
        public JsonResult Post(TicketVM tvm)
        {
            Ticket ticket = new Ticket();
            ticket.TicketNo = "ST23233";
            ticket.TicketType = tvm.TicketType;
            //////////////////////////
            
            TicketStatus ticketStatus = new TicketStatus();
            ticketStatus.TicketState = Convert.ToString(TicketState.Created);
            ticketStatus.CreationDate = DateTime.Now;
            ticketStatus.UserId = "";
            ticketStatus.TicketStatusDescription = tvm.TicketDescription;

            //////////////////////////////////
            if (ticket.TicketStatuses == null)
            {
                ticket.TicketStatuses = new Collection<TicketStatus>();
                ticket.TicketStatuses.Add(ticketStatus);
            }
            ///////////////////////////////////
            if (ticketStatus.TicketAttachments == null)
                ticketStatus.TicketAttachments = new Collection<TicketAttachment>();

            ///////////////////////////////////          
            if (!string.IsNullOrEmpty(tvm.Attachment1))
            {
                TicketAttachment ticketAttachment1 = new TicketAttachment();
                ticketStatus.TicketAttachments.Add(ticketAttachment1);
            }
            if (!string.IsNullOrEmpty(tvm.Attachment2))
            {
                TicketAttachment ticketAttachment2 = new TicketAttachment();
                ticketStatus.TicketAttachments.Add(ticketAttachment2);

            }
            if (!string.IsNullOrEmpty(tvm.Attachment3))
            {
                TicketAttachment ticketAttachment3 = new TicketAttachment();
                ticketStatus.TicketAttachments.Add(ticketAttachment3);
            }
            /////////////////////////////////////
            _authDbContext.Add(ticket);
            _authDbContext.SaveChanges();

            string query = @" WITH LastTicketStatus AS (
                              SELECT TS.*,T.TicketNo,T.TicketType, ROW_NUMBER() OVER (PARTITION BY TS.ticketid ORDER BY creationdate DESC) AS RowNo
                              FROM TicketStatus AS TS INNER JOIN Ticket T ON T.TicketId = TS.TicketId 
                            )
                            SELECT * FROM LastTicketStatus LS WHERE LS.RowNo = 1";

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
            return new JsonResult(table);
        }
    }
}
