using Core.Data;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly AuthDbContext _authDbContext;
        string userIdx = string.Empty;


        public TicketController(IConfiguration configuration,IWebHostEnvironment environment,AuthDbContext authDbContext)
        {
            _configuration = configuration;
            _env = environment;
            _authDbContext = authDbContext;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
             userIdx = User.FindFirst(ClaimTypes.Name)?.Value.Split(";")[1];

            //کارشناس میتواند صرفا تیکت های خود را رویت کند-u:dastan-role:expert
            //برنامه نویس قادر است تیکت های پروژه خود را رویت کند-u:ghadir-role:developer
            //رول ادمین قادر است تمامی تیکت ها ی پروژه های مختلف را رویت کند-u:admin-role:admin
            //رول مدیر قادر است تیکت های  سایت خود را رویت کند-u:mohiti-role:manager
            //string expertList = string.Empty;
            //if (User.IsInRole("expert"))
            //    expertList = "AND USERID = '" + userIdx + "'";
            string query = @" WITH LastTicketStatus AS (
                              SELECT TS.TicketStatusId,TS.TicketState,
                                     TS.TicketStatusDescription,FORMAT(TS.CreationDate, 'yyyy/MM/dd-HH:mm:ss', 'fa')  AS CreationDate,
                                     T.TicketId,T.TicketNo,T.TicketType,U.UserName, ROW_NUMBER() OVER (PARTITION BY TS.ticketid ORDER BY creationdate DESC) AS RowNo
                              FROM TicketStatus AS TS INNER JOIN Ticket T ON T.TicketId = TS.TicketId 
                                INNER JOIN   AspNetUsers U ON TS.UserId = U.Id
                            )
                            SELECT * FROM LastTicketStatus LS WHERE LS.RowNo = 1 ";

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
        [Authorize(Roles = "expert,developer")]
        public JsonResult Post(TicketVM tvm)
        {
            //todo:
            //make this part as stored procedure
             userIdx = User.FindFirst(ClaimTypes.Name)?.Value.Split(";")[1];

            var lastTicket = _authDbContext.Ticket.Where(r => r.TicketNo.StartsWith("ST")).OrderByDescending(x => x.TicketId).First();
            string newTicketNo = "ST" + (int.Parse(lastTicket.TicketNo.Substring(2)) + 1).ToString();
            Ticket ticket = new Ticket();
            ticket.TicketNo = newTicketNo;
            ticket.TicketType = tvm.TicketType;
            //////////////////////////
            
            TicketStatus ticketStatus = new TicketStatus();
            ticketStatus.TicketState = Convert.ToString(TicketState.Created);
            ticketStatus.CreationDate = DateTime.Now;
            ticketStatus.UserId = userIdx;
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
            if (!string.IsNullOrEmpty(tvm.Attachment))
            {
                string[] attachment =  tvm.Attachment.Split(";-");
                foreach (string file in attachment)
                {
                    TicketAttachment ticketAttachment = new TicketAttachment();
                    ticketAttachment.AttachmentPath = file;
                    ticketStatus.TicketAttachments.Add(ticketAttachment);
                }
            }
           
            /////////////////////////////////////
            _authDbContext.Add(ticket);
            _authDbContext.SaveChanges();
            string expertList = string.Empty;
            //if (User.IsInRole("expert"))
            //    expertList = "AND USERID = '" + userIdx + "'";
            //string query = @" WITH LastTicketStatus AS (
            //                  SELECT TS.*,T.TicketNo,T.TicketType, ROW_NUMBER() OVER (PARTITION BY TS.ticketid ORDER BY creationdate DESC) AS RowNo
            //                  FROM TicketStatus AS TS INNER JOIN Ticket T ON T.TicketId = TS.TicketId 
            //                )
            //                SELECT * FROM LastTicketStatus LS WHERE LS.RowNo = 1 "+expertList;

            //DataTable table = new DataTable();
            //string sqlDataSource = _configuration.GetConnectionString("SQLServerConnection");
            //SqlDataReader myReader;

            //using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            //{
            //    myCon.Open();
            //    using (SqlCommand myCommand = new SqlCommand(query, myCon))
            //    {
            //        myReader = myCommand.ExecuteReader();
            //        table.Load(myReader);
            //        myReader.Close();
            //        myCon.Close();
            //    }
            //}
            //return new JsonResult(table);
            return new JsonResult("Succeed.");
        }

        [HttpPut]
        [Authorize(Roles = "expert,developer")]
        public JsonResult Put(TicketVM tvm)
        {
            //////////////////////////
            userIdx = User.FindFirst(ClaimTypes.Name)?.Value.Split(";")[1];
            TicketStatus ticketStatus = new TicketStatus();
            ticketStatus.TicketState = Convert.ToString(tvm.TicketState);
            ticketStatus.CreationDate = DateTime.Now;
            ticketStatus.UserId = userIdx;
            ticketStatus.TicketStatusDescription = tvm.TicketDescription;
            ticketStatus.TicketId = tvm.TicketId;
            //////////////////////////////////
            _authDbContext.Add(ticketStatus);
            _authDbContext.SaveChanges();

            return new JsonResult("Updated Successfully");
        }
        [HttpGet("GetTicketDtl")]
        public JsonResult GetTicketDtl(int tId)
         {        
            string query = @" WITH LastTicketStatus AS (
                              SELECT TS.TicketStatusId,TS.TicketState,
                                     TS.TicketStatusDescription,FORMAT(TS.CreationDate, 'yyyy/MM/dd-HH:mm:ss', 'fa')  AS CreationDate,
                                     T.TicketId,U.UserName, ROW_NUMBER() OVER (PARTITION BY TS.ticketid ORDER BY creationdate DESC) AS RowNo
                              FROM TicketStatus AS TS INNER JOIN Ticket T ON T.TicketId = TS.TicketId 
                                INNER JOIN   AspNetUsers U ON TS.UserId = U.Id
                            )
                            SELECT * FROM LastTicketStatus LS WHERE TicketId =  " + tId;

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
