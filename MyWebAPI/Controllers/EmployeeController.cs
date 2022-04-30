using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyWebAPI.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            //string query
            string query = @"select EmployeeId, EmployeeName,Department,
                            convert(varchar(10),DateOfJoining,120) as DateOfJoining,PhotoFileName
                            from
                            dbo.Employee
                            ";

            //table
            DataTable table = new DataTable();

            //connection
            string sqlDataSource = _configuration.GetConnectionString("MyDB");

            //connect->command->read->load table->close
            SqlDataReader myReader;
            using(SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using(SqlCommand myCommand = new SqlCommand(query, myConn))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Employee em)
        {
            //string query
            string query = @"
                            SET IDENTITY_INSERT dbo.Employee ON
                            insert into dbo.Employee
                            (EmployeeName, Department, DateOfJoining, PhotoFileName)
                            values (@EmployeeName, @Department, @DateOfJoining, @PhotoFileName)
                            SET IDENTITY_INSERT dbo.Employee OFF
                            ";

            //table
            DataTable table = new DataTable();

            //connection
            string sqlDataSource = _configuration.GetConnectionString("MyDB");

            //connect->command->read->load table->close
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myConn))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeName", em.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", em.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", em.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", em.PhotoFileName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }

            return new JsonResult("Posted Successfully!");
        }

        [HttpPut]
        public JsonResult Put(Employee em)
        {
            //string query
            string query = @"
                            update dbo.Employee
                            set 
                            EmployeeName=@EmployeeName,
                            Department=@Department,
                            DateOfJoining=@DateOfJoining,
                            PhotoFileName=@PhotoFileName
                            where EmployeeId=@EmployeeId
                            ";

            //table
            DataTable table = new DataTable();

            //connection
            string sqlDataSource = _configuration.GetConnectionString("MyDB");

            //connect->command->read->load table->close
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myConn))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeId", em.EmployeeId);
                    myCommand.Parameters.AddWithValue("@EmployeeName", em.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", em.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", em.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", em.PhotoFileName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }

            return new JsonResult("Posted Successfully!");
        }

        [HttpDelete("{Id}")]
        public JsonResult Delete(int Id)
        {
            //string query
            string query = @"
                            delete from dbo.Employee
                            where EmployeeId=@EmployeeId
                            DBCC CHECKIDENT (Employee.EmployeeId, RESEED, 1);
                            ";

            //table
            DataTable table = new DataTable();

            //connection
            string sqlDataSource = _configuration.GetConnectionString("MyDB");

            //connect->command->read->load table->close
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myConn))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeId", Id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }

            return new JsonResult("Deleted Successfully!");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;
                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(filename);
            }
            catch
            {
                return new JsonResult("anonymous.png");
            }
        }

    }
}
