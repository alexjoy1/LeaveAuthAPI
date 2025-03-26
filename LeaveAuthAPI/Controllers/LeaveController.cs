using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class LeaveController(IConfiguration configuration) : ControllerBase
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

    // ✅ Get all leave requests
    [HttpGet]
    public async Task<ActionResult<List<LeaveRequest>>> GetLeaveRequests()
    {
        var leaveRequests = new List<LeaveRequest>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "SELECT Id, EmployeeUsername, StartDate, EndDate, Status FROM LeaveRequests";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                leaveRequests.Add(new LeaveRequest
                {
                    Id = reader.GetInt32(0),
                    EmployeeUsername = reader.GetString(1),
                    StartDate = reader.GetDateTime(2),
                    EndDate = reader.GetDateTime(3),
                    Status = reader.GetString(4)
                });
            }
        }
        return Ok(leaveRequests);
    }

    // ✅ Apply for leave
    [HttpPost]
    public async Task<IActionResult> ApplyLeave([FromBody] LeaveRequest request)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO LeaveRequests (EmployeeUsername, StartDate, EndDate, Status) VALUES (@EmployeeUsername, @StartDate, @EndDate, @Status)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@EmployeeUsername", request.EmployeeUsername);
                cmd.Parameters.AddWithValue("@StartDate", request.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", request.EndDate);
                cmd.Parameters.AddWithValue("@Status", "Pending");

                conn.Open();
                await cmd.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {


        }


        return Ok();
    }

    // ✅ Approve leave
    [HttpPut("{id}")]
    public async Task<IActionResult> ApproveLeave(int id)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "UPDATE LeaveRequests SET Status = 'Approved' WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0) return NotFound(); // If no row was updated, return 404
        }
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeave(int id)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "DELETE FROM LeaveRequests WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0) return NotFound(); // If no row was deleted, return 404
        }
        return Ok();
    }
}
public class LeaveRequest
{
    public int Id { get; set; }
    public string? EmployeeUsername { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Pending";
}
