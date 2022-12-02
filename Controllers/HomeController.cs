using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dornel.Models;
using System.Data.SqlClient;

namespace dornel.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger,
                IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index(String busca)
    {
        var queryString = "SELECT * FROM visuais";
        if(busca != null && busca != String.Empty)
        {
            queryString += $" WHERE visualnome LIKE '%{busca}%' OR visualdesc LIKE '%{busca}%' OR visualkeywords LIKE '%{busca}%'";
        }
        List<Visual> visuais = new List<Visual>();
        var connstring = _configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection connection = new SqlConnection(connstring))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    try
                    {
                        var visual = new Visual();
                        visual.visualid = (int)reader["visualid"];
                        visual.visualnome = (string)((Convert.IsDBNull(reader["visualnome"]))?String.Empty:reader["visualnome"]);
                        visual.visualdesc = (string)((Convert.IsDBNull(reader["visualdesc"]))?String.Empty:reader["visualdesc"]);
                        visual.visualdata = Convert.ToDateTime(reader["visualdata"]);
                        visual.visualkeywords = visual.visualurl = (string)((Convert.IsDBNull(reader["visualkeywords"]))?String.Empty:reader["visualkeywords"]);
                        visual.visualurl = (string)((Convert.IsDBNull(reader["visualurl"]))?String.Empty:reader["visualurl"]);
                        visual.miniatura = (string)((Convert.IsDBNull(reader["miniatura"]))?"/placeholder.png":reader["miniatura"]);
                        visuais.Add(visual);
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        return View(visuais);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
